using Domain.Models.Nominatim;
using MyExpenses.WebApi.Entities;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.WebApi.Mappings;

[Mapper]
public static partial class NominatimNominatimMapper
{
    public static partial NominatimSearchResultDomain MapToDomain(this NominatimSearchResult entity);

    public static partial NominatimDetailedAddressDomain MapToDomain(this NominatimDetailedAddress entity);

    [MapDerivedType(typeof(NominatimPoint), typeof(NominatimPointDomain))]
    [MapDerivedType(typeof(NominatimLineString), typeof(NominatimLineStringDomain))]
    [MapDerivedType(typeof(NominatimPolygon), typeof(NominatimPolygonDomain))]
    public static partial NominatimGeoJsonDomain MapToDomain(this NominatimGeoJson entity);
}