using AutoMapper;
using Mapsui.Layers;
using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Models.AutoMapper.Profiles;

public class PointFeatureToTPlaceProfile : Profile
{
    public PointFeatureToTPlaceProfile()
    {
        CreateMap<PointFeature, TPlace>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["id"]))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src["name"]))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src["number"]))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src["street"]))
            .ForMember(dest => dest.Postal, opt => opt.MapFrom(src => src["postal"]))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src["city"]))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src["country"]))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src["latitude"]))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src["longitude"]))
            .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => src["date_added"]))
            .ReverseMap();
    }
}