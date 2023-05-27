using Entities.Models;
using System.Threading.Tasks;

namespace Domain.Database.Redis
{
    public interface IRedisSaveDataStrategy: ISaveDataStrategy
    {
        Task<PersonEntity> Execute(PersonEntity person);
    }
}