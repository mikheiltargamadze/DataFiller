using AutoMapper;
using System.Collections.Generic;

namespace ViewModels.AutoMapepr
{
    public class CustomMappingProfile : AutoMapper.Profile
    {
        public CustomMappingProfile(IEnumerable<IHaveCustomMapping> haveCustomMappings)
        {
            foreach (var item in haveCustomMappings)
                item.CreateMappings(this);
        }
    }
}
