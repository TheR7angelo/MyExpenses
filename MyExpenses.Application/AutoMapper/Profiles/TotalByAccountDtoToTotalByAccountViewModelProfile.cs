using AutoMapper;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.ViewModels.Accounts;

namespace MyExpenses.Application.AutoMapper.Profiles;

public class TotalByAccountDtoToTotalByAccountViewModelProfile : Profile
{
    public TotalByAccountDtoToTotalByAccountViewModelProfile()
    {
        CreateMap<TotalByAccountDto, TotalByAccountViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.TotalPointed, opt => opt.MapFrom(src => src.TotalPointed))
            .ForMember(dest => dest.TotalNotPointed, opt => opt.MapFrom(src => src.TotalNotPointed))
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
            .ReverseMap();
    }
}