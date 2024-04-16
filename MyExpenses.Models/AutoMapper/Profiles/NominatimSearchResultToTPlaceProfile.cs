using AutoMapper;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.WebApi.Nominatim;

namespace MyExpenses.Models.AutoMapper.Profiles;

public class NominatimSearchResultToTPlaceProfile : Profile
{
    public NominatimSearchResultToTPlaceProfile()
    {
        CreateMap<NominatimSearchResult, TPlace>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Number,
                opt => opt.MapFrom(src => src.Address != null ? src.Address.HouseNumber : null))
            .ForMember(dest => dest.Street,
                opt => opt.MapFrom(src =>
                    src.Address != null
                        ? !string.IsNullOrEmpty(src.Address.Road) && !string.IsNullOrEmpty(src.Address.Suburb)
                            ? $"{src.Address.Road}, {src.Address.Suburb}"
                            : src.Address.Road ?? src.Address.Suburb
                        : null))
            .ForMember(dest => dest.Postal,
                opt => opt.MapFrom(src => src.Address != null ? src.Address!.Postcode : null))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address != null ? src.Address!.City : null))
            .ForMember(dest => dest.Country,
                opt => opt.MapFrom(src => src.Address != null ? src.Address!.Country : null))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
            .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(_ => DateTime.Now))
            ;
    }
}