using Common;

namespace WebFramework.RabbitMQ
{
    public interface IRpcClientQueue:IScopedDependency
    {
        void Get();
    }
}