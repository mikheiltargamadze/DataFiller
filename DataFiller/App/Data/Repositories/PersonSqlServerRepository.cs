using Dapper;
using Data.Contracts;
using Entities;
using Entities.Models;
using Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{

    public class PersonSqlServerRepository : IPersonSqlRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        public PersonSqlServerRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }
        public async Task<PersonEntity> Add(PersonEntity entity)
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();
            var inserted = await connection.QuerySingleAsync<PersonEntity>(DatabaseStoreProcedure.InsertPerson, new { entity.Age, entity.FirstName, entity.LastName }, commandType: System.Data.CommandType.StoredProcedure);
            return inserted;
        }

        public async Task<int> Delete(int id)
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();
            var affectedRows = await connection.ExecuteAsync(DatabaseStoreProcedure.DeletePerson, new { Id = id }, commandType: System.Data.CommandType.StoredProcedure);
            return affectedRows;
        }

        public async Task<PersonEntity> Get(int id)
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();
            var result = await connection.QueryAsync<PersonEntity>(DatabaseStoreProcedure.GetPerson, new { Id = id }, commandType: System.Data.CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<PersonEntity>> GetAll()
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();
            var result = await connection.QueryAsync<PersonEntity>(DatabaseStoreProcedure.GetPeople, commandType: System.Data.CommandType.StoredProcedure);
            return result;
        }

        public async Task<PersonEntity> Update(PersonEntity entity)
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();
            var inserted = await connection.QuerySingleAsync<PersonEntity>(DatabaseStoreProcedure.UpdatePerson, entity, commandType: System.Data.CommandType.StoredProcedure);
            return inserted;
        }
    }
}
