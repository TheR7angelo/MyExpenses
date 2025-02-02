using AutoMapper;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Models.Wpf.Charts;

namespace MyExpenses.Models.AutoMapper.Profiles;

public class AnalysisVBudgetTotalAnnualToBudgetRecordInfoProfile : Profile
{
    public AnalysisVBudgetTotalAnnualToBudgetRecordInfoProfile()
    {
        CreateMap<AnalysisVBudgetTotalAnnual, BudgetRecordInfo>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.PeriodValue ?? 0))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status!))
            .ForMember(dest => dest.DifferenceValue, opt => opt.MapFrom(src => src.DifferenceValue ?? 0))
            .ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => src.Percentage ?? 0));
    }
}