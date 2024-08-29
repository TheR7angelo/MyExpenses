using AutoMapper;

namespace MyExpenses.Models.AutoMapper.Profiles.WebApi.Github;

public class HardReleaseToSoftReleaseProfile : Profile
{
    public HardReleaseToSoftReleaseProfile()
    {
        CreateMap<MyExpenses.Models.WebApi.Github.Hard.Release, Models.WebApi.Github.Soft.Release>()
            .ReverseMap();
    }
}