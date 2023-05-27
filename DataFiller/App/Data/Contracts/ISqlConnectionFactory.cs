using System.Data;

namespace Persistence
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
