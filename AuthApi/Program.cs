using AuthApi.Config;
using AuthApi.Interfaces;
using AuthApi.Repositories;
using AuthApi.Services;
using AuthApi.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Permite qualquer origem
              .AllowAnyMethod()   // Permite qualquer método HTTP (GET, POST, etc.)
              .AllowAnyHeader();  // Permite qualquer cabeçalho
    });
});
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});


builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
    
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
var app = builder.Build();
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();