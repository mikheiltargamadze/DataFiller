using Common;
using Common.Utilities;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Text;

namespace WebFramework.RabbitMQ
{
    //public interface IRabbitManager
    //{
    //    void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey)
    //        where T : class;
    //}

    public class RabbitManagerQueue : IRabbitManager
    {
        private readonly SiteSettings _siteSettings;
        private readonly string queueName;
        private readonly DefaultObjectPool<IModel> _objectPool;

        public RabbitManagerQueue(IPooledObjectPolicy<IModel> objectPolicy, IOptions<SiteSettings> siteSettings)
        {
            _siteSettings = siteSettings.Value;
            queueName = _siteSettings.RabbitMQSettings.QueueName;
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
        }

        public void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey)
            where T : class
        {
            if (message == null)
                return;

            var channel = _objectPool.Get();

            try
            {

                channel.QueueDeclare(queue: queueName,
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                //string message2 = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message.ToJson());

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);


                //channel.QueueDeclare(queue: exchangeName,
                //                durable: false,
                //                exclusive: false,
                //                autoDelete: false,
                //                arguments: null);

                //var sendBytes = Encoding.UTF8.GetBytes(message.ToJson());

                //var properties = channel.CreateBasicProperties();
                //properties.Persistent = true;

                //channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _objectPool.Return(channel);
            }
        }
    }
}
