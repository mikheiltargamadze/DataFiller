using Entities.Models;
using System.Threading.Tasks;

namespace Domain.Database
{
    public interface ISqlServerSaveDataStrategy: ISaveDataStrategy
    {
        Task<PersonEntity> Execute(PersonEntity person);
    }
}