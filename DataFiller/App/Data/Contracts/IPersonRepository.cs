using Entities.Models;

namespace Data.Contracts
{
    public interface IPersonRepository : IGenericRepository<PersonEntity>
    {
    }
    public interface IPersonRedisRepository : IPersonRepository
    {

    }
    public interface IPersonSqlRepository : IPersonRepository
    {

    }
}
