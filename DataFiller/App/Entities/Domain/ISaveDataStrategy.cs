using Entities.Models;
using System.Threading.Tasks;

namespace Domain.Database
{
    public interface ISaveDataStrategy
    {
        Task<PersonEntity> Execute(PersonEntity person);
    }
}
