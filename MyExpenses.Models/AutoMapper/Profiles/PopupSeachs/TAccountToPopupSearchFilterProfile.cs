using AutoMapper;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Models.AutoMapper.Profiles.PopupSeachs;

public class TAccountToPopupSearchFilterProfile : Profile
{
    public TAccountToPopupSearchFilterProfile()
    {
        CreateMap<TAccount, PopupSearch>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Name));
    }
}