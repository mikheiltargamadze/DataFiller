using Data.Contracts;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories
{

    public class PersonRedisRepository : IPersonRedisRepository
    {
        private readonly IRedisConnectionFactory _redisConnectionFactory;
        public PersonRedisRepository(IRedisConnectionFactory redisConnectionFactory)
        {
            _redisConnectionFactory = redisConnectionFactory;
        }
        public async Task<PersonEntity> Add(PersonEntity entity)
        {
            var connection = _redisConnectionFactory.GetOpenConnection();
            entity.Id = connection.Keys($"people*").Length;
            connection.Set($"people:Id:{entity.Id}", entity);

            return entity;
        }

        public Task<int> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PersonEntity> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PersonEntity>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<PersonEntity> Update(PersonEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
