using Common;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WebFramework.RabbitMQ
{


    public class RpcClientQueue
    {
        private readonly string _queueName;
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;
        private SiteSettings _siteSettings;

        public RpcClientQueue(IOptions<SiteSettings> siteSettings)
        {
            _siteSettings = siteSettings.Value;
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _siteSettings.RabbitMQSettings.HostName,
                    Password = _siteSettings.RabbitMQSettings.Password,
                    Port = _siteSettings.RabbitMQSettings.Port,
                    UserName = _siteSettings.RabbitMQSettings.UserName,
                    DispatchConsumersAsync = true
                };

                _queueName = _siteSettings.RabbitMQSettings.QueueName;

                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.QueueDeclare(queue: _queueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: _queueName,
                                     autoAck: true,
                                     consumer: consumer);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

                Close();
            }

        }
        //public string Get()
        //{

        //}

        private void Close()
        {
            connection.Close();
        }
    }
}
