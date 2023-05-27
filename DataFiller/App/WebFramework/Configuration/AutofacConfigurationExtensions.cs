using Autofac;
using Autofac.Core.Activators.Reflection;
using Autofac.Extensions.DependencyInjection;
using Common;
using Data;
using Data.Contracts;
using Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using WebFramework.RabbitMQ;

namespace WebFramework.Configuration
{
    internal class AllConstructorFinder : IConstructorFinder
    {
        private static readonly ConcurrentDictionary<Type, ConstructorInfo[]> Cache = new ConcurrentDictionary<Type, ConstructorInfo[]>();

        public ConstructorInfo[] FindConstructors(Type targetType)
        {
            var result = Cache.GetOrAdd(targetType, t => t.GetTypeInfo().DeclaredConstructors.ToArray());
            return result.Length > 0 ? result : throw new NoConstructorsFoundException(targetType);
        }
    }

    public class DataAccessModule : Autofac.Module
    {
        private readonly string _databaseConnectionString;
        private readonly SiteSettings _siteSetting;

        public DataAccessModule(string databaseConnectionString, SiteSettings siteSetting)
        {
            _databaseConnectionString = databaseConnectionString;
            _siteSetting = siteSetting;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SqlConnectionFactory>()
                .As<ISqlConnectionFactory>()
                .WithParameter("connectionString", _databaseConnectionString)
                .InstancePerLifetimeScope();

            builder.RegisterType<RedisConnectionFactory>()
                .As<IRedisConnectionFactory>()
                .WithParameter("host", _siteSetting.Redis.Host)
                .WithParameter("port", _siteSetting.Redis.Port)
                .InstancePerLifetimeScope();



            builder.RegisterType<PersonSqlServerRepository>()
             .As<IPersonSqlRepository>()
             .InstancePerLifetimeScope();

            builder.RegisterType<PersonRedisRepository>()
                .As<IPersonRedisRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerLifetimeScope();

        }
    }


    public class RedisConnectionFactory : IRedisConnectionFactory, IDisposable
    {

        private RedisClient _connection;
        private int _port;
        private string _host;

        public RedisConnectionFactory(string host, int port)
        {
            _port = port;
            _host = host;
        }

        public RedisClient GetOpenConnection()
        {
            _connection = new RedisClient(_host, _port);
            return _connection;
        }

        public void Dispose()
        {
            if (this._connection != null)
                this._connection.Dispose();

        }
    }

    public class SqlConnectionFactory : ISqlConnectionFactory, IDisposable
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public SqlConnectionFactory(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public IDbConnection GetOpenConnection()
        {
            if (this._connection == null || this._connection.State != ConnectionState.Open)
            {
                this._connection = new SqlConnection(_connectionString);
                this._connection.Open();
            }

            return this._connection;
        }

        public void Dispose()
        {
            if (this._connection != null && this._connection.State == ConnectionState.Open)
            {
                this._connection.Dispose();
            }
        }
    }
    public static class AutofacConfigurationExtensions
    {
        public static void AddServices(this ContainerBuilder containerBuilder)
        {
            var commonAssembly = typeof(SiteSettings).Assembly;
            var webframwork = typeof(AutofacConfigurationExtensions).Assembly;

            var webFrameworkAssembly = typeof(IRpcClientQueue).Assembly;


            containerBuilder.RegisterAssemblyTypes(commonAssembly, webframwork, webFrameworkAssembly)
                .AssignableTo<IScopedDependency>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterAssemblyTypes(commonAssembly, webframwork, webFrameworkAssembly)
                .AssignableTo<ITransientDependency>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            containerBuilder.RegisterAssemblyTypes(commonAssembly, webframwork, webFrameworkAssembly)
                .AssignableTo<ISingletonDependency>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }

        public static IServiceProvider BuildAutofacServiceProvider(this IServiceCollection services, IConfiguration configuration)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);

            containerBuilder.AddServices();

            var siteSetting = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();

            containerBuilder.RegisterModule(new DataAccessModule(configuration.GetConnectionString("SqlServer"), siteSetting));

            var container = containerBuilder.Build();

            return new AutofacServiceProvider(container);
        }
    }
}