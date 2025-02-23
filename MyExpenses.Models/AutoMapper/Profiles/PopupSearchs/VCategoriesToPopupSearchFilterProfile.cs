using AutoMapper;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Views;

namespace MyExpenses.Models.AutoMapper.Profiles.PopupSearchs;

public class VCategoriesToPopupSearchFilterProfile : Profile
{
    public VCategoriesToPopupSearchFilterProfile()
    {
        CreateMap<VCategory, PopupSearch>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.CategoryName));
    }
}