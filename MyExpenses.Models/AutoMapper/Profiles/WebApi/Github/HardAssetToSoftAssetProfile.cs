using AutoMapper;

namespace MyExpenses.Models.AutoMapper.Profiles.WebApi.Github;

public class HardAssetToSoftAssetProfile : Profile
{
    public HardAssetToSoftAssetProfile()
    {
        CreateMap<MyExpenses.Models.WebApi.Github.Hard.Asset, MyExpenses.Models.WebApi.Github.Soft.Asset>()
            .ReverseMap();
    }
}