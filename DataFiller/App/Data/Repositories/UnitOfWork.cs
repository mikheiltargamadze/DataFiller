using Common;
using Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IPersonSqlRepository personSqlRepository, IPersonRedisRepository personRedisRepository)
        {
            RedisPeople = personRedisRepository;
            SqlPeople = personSqlRepository;
        }
        public IPersonRepository RedisPeople { get; }
        public IPersonRepository SqlPeople { get; }
    }
}
