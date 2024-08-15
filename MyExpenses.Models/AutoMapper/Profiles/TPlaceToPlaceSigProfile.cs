using AutoMapper;
using MyExpenses.Models.IO.Sig.Keyhole_Markup_Language;
using TPlace = MyExpenses.Models.Sql.Bases.Tables.TPlace;

namespace MyExpenses.Models.AutoMapper.Profiles;

public class TPlaceToPlaceSigProfile : Profile
{
    public TPlaceToPlaceSigProfile()
    {
        CreateMap<TPlace, PlaceSig>()
            .ReverseMap();
    }
}