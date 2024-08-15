using AutoMapper;
using MyExpenses.Models.IO.Sig.Keyhole_Markup_Language;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Models.AutoMapper.Profiles;

public class TPlaceToPlaceSigProfile : Profile
{
    public TPlaceToPlaceSigProfile()
    {
        CreateMap<TPlace, PlaceSig>()
            .ReverseMap();
    }
}