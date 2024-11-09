using AutoMapper;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Derivatives.Tables;

namespace MyExpenses.Models.AutoMapper.Profiles.WebApi;

public class TPlaceToTPlaceDeriveProfile : Profile
{
    public TPlaceToTPlaceDeriveProfile()
    {
        CreateMap<TPlace, TPlaceDerive>();
    }
}