using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ReleaseAPI.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ReleaseAPI.Utils;
using ReleaseAPI.Interfaces;
using ReleaseApi.Repositories;
using AuthApi.Interfaces;
using ReleaseAPI.Services;
using Amazon.SQS;

namespace ReleaseAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "console")
            {
                var builder = Host.CreateDefaultBuilder(args);
                builder.ConfigureServices((hostContext, services) =>
                {   
                    var configuration = hostContext.Configuration;
                    services.Configure<QueueSettings>(configuration.GetSection("SQSSettings"));
                    services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
                    AddServices(services);
                    services.AddHostedService<QueueProcessingService>(); // Serviço que irá processar a fila
                });
                builder.Build().Run();
            } else {            
                var builder = WebApplication.CreateBuilder(args);

                builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection("SQSSettings"));
                builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

                AddServices(builder.Services);
                var app = builder.Build();
                app.UseMiddleware<ExceptionMiddleware>();
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();
                app.Run();
            }
        }

        private static void AddServices(IServiceCollection service) {
            

            service.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<QueueSettings>>().Value;
                return new AmazonSQSClient("test", "test", new AmazonSQSConfig
                {
                    ServiceURL = settings.ServiceUrl
                });
            });

            service.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            service.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(settings.DatabaseName);
            });


            service.AddControllers();

            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "SeuIssuer",  // Defina o emissor esperado
                        ValidAudience = "SeuAudience",
                        IssuerSigningKey = new RsaSecurityKey(KeyManager.LoadPublicKey(Path.Combine(AppContext.BaseDirectory, "public.key"))),
                        ClockSkew = TimeSpan.Zero // Para evitar problemas com a expiração do token
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // Log aqui para verificar a falha na autenticação
                            Console.WriteLine($"Falha na autenticação: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            // Log para quando o token for validado com sucesso
                            Console.WriteLine("Token validado com sucesso!");
                            return Task.CompletedTask;
                        }
                    };
                });
            service.AddSingleton<IReleaseRepository, ReleaseRepository>();
            service.AddSingleton<IQueueService, QueueService>();
            service.AddScoped<IReleaseService, ReleaseService>();
            service.AddHttpContextAccessor();
        }
    }

    public class QueueProcessingService(IQueueService queueService) : IHostedService
    {
        private readonly IQueueService _queueService = queueService;
        private CancellationTokenSource? _cancellationTokenSource;
         // Inicia o processamento de fila
        
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try {
                    await _queueService.ListenForMessagesAsync();
                } catch(Exception err) {
                    Console.WriteLine(err);
                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.Cancel();
            return Task.CompletedTask;
        }
    }
}