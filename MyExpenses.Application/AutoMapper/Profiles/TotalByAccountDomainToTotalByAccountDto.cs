using AutoMapper;
using Domain.Models.Accounts;
using MyExpenses.Application.Models.Accounts;

namespace MyExpenses.Application.AutoMapper.Profiles;

public class TotalByAccountDomainToTotalByAccountDto : Profile
{
    public TotalByAccountDomainToTotalByAccountDto()
    {
        CreateMap<TotalByAccountDomain, TotalByAccountDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.TotalPointed, opt => opt.MapFrom(src => src.TotalPointed))
            .ForMember(dest => dest.TotalNotPointed, opt => opt.MapFrom(src => src.TotalNotPointed))
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol));
    }
}