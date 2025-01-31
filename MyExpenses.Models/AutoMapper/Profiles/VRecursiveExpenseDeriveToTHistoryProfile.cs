using AutoMapper;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Derivatives.Views;

namespace MyExpenses.Models.AutoMapper.Profiles;

public class VRecursiveExpenseDeriveToTHistoryProfile : Profile
{
    public VRecursiveExpenseDeriveToTHistoryProfile()
    {
        CreateMap<VRecursiveExpenseDerive, THistory>()
            .ForMember(dest => dest.RecursiveExpenseFk, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DateAdded, opt => opt.Ignore());
    }
}