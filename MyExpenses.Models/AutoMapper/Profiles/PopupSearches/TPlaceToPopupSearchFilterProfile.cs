using AutoMapper;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Models.AutoMapper.Profiles.PopupSearches;

public class TPlaceToPopupSearchFilterProfile : Profile
{
    public TPlaceToPopupSearchFilterProfile()
    {
        CreateMap<TPlace, PopupSearch>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Name));
    }
}