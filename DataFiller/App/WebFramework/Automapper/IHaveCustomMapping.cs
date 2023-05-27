using AutoMapper;

namespace ViewModels.AutoMapepr
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(AutoMapper.Profile profile);
    }
}
