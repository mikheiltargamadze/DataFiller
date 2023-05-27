using ServiceStack.Redis;

namespace Data
{
    public interface IRedisConnectionFactory
    {
        RedisClient GetOpenConnection();
    }
}
