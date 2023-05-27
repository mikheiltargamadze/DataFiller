using Common;
using Common.Utilities;
using Domain.Database;
using Domain.Database.Redis;
using Entities.Models;
using Infrastructure.BackgroundWorks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebFramework.BackgroundWorks
{
    public class ListenToServiceWorker : ScopedBackgroundService
    {
        private readonly SiteSettings _siteSettings;
        private readonly ILogger<ListenToServiceWorker> _logger;
        private readonly string _queueName;
        private readonly DefaultObjectPool<IModel> _objectPool;
        private IModel channel;
        private AsyncEventingBasicConsumer consumer;
        private List<ISaveDataStrategy> Databases;


        public ListenToServiceWorker(ILogger<ListenToServiceWorker> logger
            , IRedisSaveDataStrategy redis
            , ISqlServerSaveDataStrategy sqlServer,
            IOptions<SiteSettings> siteSettings, IServiceScopeFactory serviceScopeFactory
            , IPooledObjectPolicy<IModel> objectPolicy
            ) : base(serviceScopeFactory)
        {

            Databases = new List<ISaveDataStrategy>();
            Databases.AddRange(new ISaveDataStrategy[] { sqlServer, redis });


            _siteSettings = siteSettings.Value;
            _logger = logger;
            _queueName = _siteSettings.RabbitMQSettings.QueueName;

            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);

            channel = _objectPool.Get();

            Log.Write(_logger, "Listen To RabbitMQ");

            channel.QueueDeclare(queue: _queueName,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            consumer = new AsyncEventingBasicConsumer(channel);

        }

        public override Task ExecuteInScope(IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    string message = ReadMessage(ea);
                    var person = message.FromJson<Person>();
                    var entity = person.ToEntity();
                    foreach (var database in Databases)
                        entity=await database.Execute(entity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToJson());
                }
            };
            channel.BasicConsume(queue: _queueName,
                                 autoAck: true,
                                 consumer: consumer);

            return Task.CompletedTask;
        }
        private string ReadMessage(BasicDeliverEventArgs ea)
        {
            _logger.LogInformation("Received From RabbitMQ");
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            return message;
        }
    }
}
