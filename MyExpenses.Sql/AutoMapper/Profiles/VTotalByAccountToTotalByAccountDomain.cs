using AutoMapper;
using Domain.Models.Accounts;
using MyExpenses.Models.Sql.Bases.Views;

namespace MyExpenses.Sql.AutoMapper.Profiles;

public class VTotalByAccountToTotalByAccountDomain : Profile
{
    public VTotalByAccountToTotalByAccountDomain()
    {
        CreateMap<VTotalByAccount, TotalByAccountDomain>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? "Unknown"))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total ?? 0.0))
            .ForMember(dest => dest.TotalPointed, opt => opt.MapFrom(src => src.TotalPointed ?? 0.0))
            .ForMember(dest => dest.TotalNotPointed, opt => opt.MapFrom(src => src.TotalNotPointed ?? 0.0))
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol ?? "$"));
    }
}