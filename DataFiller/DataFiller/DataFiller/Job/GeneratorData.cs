using Common.Utilities;
using DataFiller.Model;
using Quartz;
using System;
using System.Threading.Tasks;
using WebFramework.RabbitMQ;

namespace DataFiller.Job
{
    [DisallowConcurrentExecution]
    class GeneratorData : IJob
    {
        private IRabbitManager _rabbitManager;

        public GeneratorData(IRabbitManager rabbitManager)
        {
            _rabbitManager = rabbitManager;
        }
        public Task Execute(IJobExecutionContext context)
        {
            //add to rabbitMq
            var person = new Person("mikheil", "targamadze", 33);
            _rabbitManager.Publish<Person>(person, "direct_logs", "direct", "");
            Console.WriteLine(person.ToJson());
            return Task.CompletedTask;
        }
    }
}
