using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models;
using AzureStorage.Wrappers;

namespace AzureStorage.Services
{
    public class QueueStorage
    {
        private readonly string connectionStringStorage;
        
        public QueueStorage(IConfiguration configuration)
        {
            connectionStringStorage = configuration.GetSection("ConnectionStrings:AzureStorage").Value;
        }

        public async Task<Response<PeekedMessage[]>> Peek(string queueName)
        {
            QueueClient queueClient = await QueueClient(queueName);
            PeekedMessage[] messages = await queueClient.PeekMessagesAsync(queueClient.MaxPeekableMessages);

            return new Response<PeekedMessage[]>(messages);
        }

        public async Task<Response<string>> Add(string queueName, string message, TimeSpan? visibilityTimeout = null, TimeSpan? timeToLive = null) 
        {
            QueueClient queueClient = await QueueClient(queueName);
            SendReceipt sendReceipt = await queueClient.SendMessageAsync(message, visibilityTimeout, timeToLive);

            return new Response<string>(data: sendReceipt.MessageId);

        }

        public async Task<Response<bool>> Update(string queueName, string messageText, TimeSpan? visibilityTimeout = null)
        {
            QueueClient queueClient = await QueueClient(queueName);
            QueueMessage queueMessage = await queueClient.ReceiveMessageAsync();
            if (queueMessage == null)
            {
                return new Response<bool>("Not found message");
            }
            await queueClient.UpdateMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt, messageText, visibilityTimeout ?? TimeSpan.FromDays(7));

            return new Response<bool>(true);
        }

        public async Task<Response<int>> Delete(string queueName)
        {
            QueueClient queueClient = await QueueClient(queueName);
            QueueMessage queueMessage = await queueClient.ReceiveMessageAsync();
            if (queueMessage == null)
            {
                return new Response<int>("Not found message");
            }
            var delete = await queueClient.DeleteMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt);

            return new Response<int>(delete.Status);
        }


        private async Task<QueueClient> QueueClient(string queueName) 
        {

            QueueClient queueClient = new QueueClient(connectionStringStorage, queueName);

            await queueClient.CreateIfNotExistsAsync();

            return queueClient;
        }

    }
}
