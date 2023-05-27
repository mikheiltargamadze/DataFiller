using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFiller.Job
{
    public class JobSchedule
    {
        public JobSchedule(Type jobType,string cornExpression)
        {
            JobType = jobType;
            CronExpression = cornExpression;
        }
        public Type JobType { get; set; }
        public string CronExpression { get; set; }
    }
}
