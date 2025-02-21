

using Amazon.SQS;
using Amazon.SQS.Model;
using AuthApi.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReleaseAPI.Config;
using ReleaseAPI.Interfaces;

namespace ReleaseAPI.Services 
{
    public class QueueService(
        AmazonSQSClient sqsClient,
        IReleaseRepository releaseRepository,
        IOptions<QueueSettings> queueSettings
            ) : IQueueService
    {
        private readonly AmazonSQSClient _sqsClient = sqsClient;
        private readonly QueueSettings _queueSettings = queueSettings.Value;
        private readonly IReleaseRepository _releaseRepository = releaseRepository;

        public async Task<string> SendMessage<T>(T param) 
        {
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = _queueSettings.QueueUrl,
                MessageBody = JsonConvert.SerializeObject(param)
            };
            
            var response = await _sqsClient.SendMessageAsync(sendMessageRequest);            
            return response.MessageId;
        }

        public async Task ListenForMessagesAsync()
        {
            try
            {
                var receiveMessageRequest = new ReceiveMessageRequest
                {
                    QueueUrl = _queueSettings.QueueUrl,
                    MaxNumberOfMessages = 10, // Número máximo de mensagens por vez
                    WaitTimeSeconds = 20,     // Long Polling: tempo de espera antes de retornar
                };

                // Recebe as mensagens da fila
                var response = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest);
                // Processa as mensagens se existirem
                if (response.Messages.Count > 0)
                {
                    foreach (var message in response.Messages)
                    {
                        await ProcessMessageAsync(message);
                        await DeleteMessageAsync(message);
                    }
                }
                else
                {
                    Console.WriteLine("No messages received.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
        }

        private async Task ProcessMessageAsync(Message message)
        {
            var release = JsonConvert.DeserializeObject<ReleaseApi.Domain.Entities.Release>(message.Body)!;
            await _releaseRepository.CreateRelease(release);
        }
        private async Task DeleteMessageAsync(Message message)
        {
            var deleteMessageRequest = new DeleteMessageRequest
            {
                QueueUrl = _queueSettings.QueueUrl,
                ReceiptHandle = message.ReceiptHandle
            };
            await _sqsClient.DeleteMessageAsync(deleteMessageRequest);
        }

    }
}