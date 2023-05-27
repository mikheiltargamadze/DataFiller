using Common;
using Common.Utilities;
using Data.Contracts;
using Domain.Database;
using Entities.Models;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Threading.Tasks;

namespace Services.Database.Sql
{
    public class SqlServerSaveDataStrategy : ISqlServerSaveDataStrategy, ITransientDependency
    {
        private IUnitOfWork _unitOfWork;
        private ILogger<SqlServerSaveDataStrategy> _logger;

        public SqlServerSaveDataStrategy(ILogger<SqlServerSaveDataStrategy> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<PersonEntity> Execute(PersonEntity person)
        {
            Log.Write(_logger, MethodBase.GetCurrentMethod(),person);
            person = await _unitOfWork.SqlPeople.Add(person);
            Log.Write(_logger, $"Added To SqlServer people:Id:{person.Id} => {person.ToJson()}");
            return person;
        }
    }
}
