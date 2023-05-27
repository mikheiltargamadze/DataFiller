using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace ViewModels.AutoMapepr
{
    public static class AutoMapperConfiguration
    {
        public static void InitializeAutoMapper()
        {
            Mapper.Initialize(config =>
            {
                config.AddCustomMappingProfile();
            });

            //Compile mapping after configuration to boost map speed
            Mapper.Configuration.CompileMappings();
        }

        public static void AddCustomMappingProfile(this IMapperConfigurationExpression config)
        {
            var viewModelsAssembly = typeof(IHaveCustomMapping).Assembly;
            config.AddCustomMappingProfile(Assembly.GetEntryAssembly(), viewModelsAssembly);
        }

        public static void AddCustomMappingProfile(this IMapperConfigurationExpression config, params Assembly[] assemblies)
        {
            var allTypes = assemblies.SelectMany(a => a.ExportedTypes);

            var list = allTypes.Where(type => type.IsClass && !type.IsAbstract &&
                type.GetInterfaces().Contains(typeof(IHaveCustomMapping)))
                .Select(type => (IHaveCustomMapping)Activator.CreateInstance(type));

            var profile = new CustomMappingProfile(list);

            config.AddProfile(profile);
        }
    }
}
