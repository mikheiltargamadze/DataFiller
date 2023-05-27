using Autofac;
using Common;
using Common.Utilities;
using Data;
using Data.Contracts;
using Data.Repositories;
using Domain.Database;
using Domain.Database.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Database.Redis;
using Services.Database.Sql;
using System.Reflection;
using ViewModels.AutoMapepr;
using WebFramework.BackgroundWorks;
using WebFramework.Configuration;
using WebFramework.MiddleWares;
using WebFramework.RabbitMQ;

namespace DataFiller
{
    public class Startup
    {
        private readonly SiteSettings _siteSetting;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            AutoMapperConfiguration.InitializeAutoMapper();

            _siteSetting = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISqlConnectionFactory>(new SqlConnectionFactory(Configuration.GetConnectionString("SqlServer")));
            services.AddSingleton<IRedisConnectionFactory>(new RedisConnectionFactory(_siteSetting.Redis.Host,_siteSetting.Redis.Port));
            services.AddTransient<IPersonSqlRepository, PersonSqlServerRepository>();
            services.AddTransient<IPersonRedisRepository, PersonRedisRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IRedisSaveDataStrategy, RedisSaveDataStrategy>();
            services.AddTransient<ISqlServerSaveDataStrategy, SqlServerSaveDataStrategy>();

            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));

            services.AddHostedService<ListenToServiceWorker>();
            services.BuildAutofacServiceProvider(Configuration);
            services.AddRabbit(Configuration, _siteSetting.RabbitMQSettings);
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCustomExceptionHandler();
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.RegisterType<QuartzWrapper>().As<IQuartzWrapper>().SingleInstance();
            //builder.Register(provider => new JobFactory(provider)).As<IJobFactory>().InstancePerDependency();
            //builder.RegisterType<JobClass>().As<IJob>().InstancePerDependency();
        }

    }
}
