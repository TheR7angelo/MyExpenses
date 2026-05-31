using MyExpenses.Application.Dtos.Nominatium;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.ViewModels.Locations;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Presentation.Mappings;

[Mapper]
public partial class NominatimDtoViewModelMapper : INominatimDtoViewModelMapper
{
    public partial NominatimSearchResultViewModel MapToViewModel(NominatimSearchResultDto src);

    [MapperIgnoreTarget(nameof(NominatimDetailedAddressViewModel.CityName))]
    public partial NominatimDetailedAddressViewModel MapToViewModel(NominatimDetailedAddressDto src);

    [MapDerivedType(typeof(NominatimPointDto), typeof(NominatimPointViewModel))]
    [MapDerivedType(typeof(NominatimLineStringDto), typeof(NominatimLineStringViewModel))]
    [MapDerivedType(typeof(NominatimPolygonDto), typeof(NominatimPolygonViewModel))]
    public partial NominatimGeoJsonViewModel MapToViewModel(NominatimGeoJsonDto src);

    [MapperIgnoreSource(nameof(NominatimSearchResultViewModel.PlaceId))]
    [MapperIgnoreSource(nameof(NominatimSearchResultViewModel.Licence))]
    [MapperIgnoreSource(nameof(NominatimSearchResultViewModel.OsmType))]
    [MapperIgnoreSource(nameof(NominatimSearchResultViewModel.OsmId))]
    [MapperIgnoreSource(nameof(NominatimSearchResultViewModel.Class))]
    [MapperIgnoreSource(nameof(NominatimSearchResultViewModel.Type))]
    [MapperIgnoreSource(nameof(NominatimSearchResultViewModel.PlaceRank))]
    [MapperIgnoreSource(nameof(NominatimSearchResultViewModel.Importance))]
    [MapperIgnoreSource(nameof(NominatimSearchResultViewModel.AddressType))]
    [MapperIgnoreSource(nameof(NominatimSearchResultViewModel.DisplayName))]
    [MapperIgnoreSource(nameof(NominatimSearchResultViewModel.BoundingBox))]
    [MapperIgnoreSource(nameof(NominatimSearchResultViewModel.GeoJson))]
    [MapperIgnoreTarget(nameof(PlaceViewModel.Id))]
    [MapperIgnoreTarget(nameof(PlaceViewModel.IsOpen))]
    [MapperIgnoreTarget(nameof(PlaceViewModel.CanBeDeleted))]
    [MapperIgnoreTarget(nameof(PlaceViewModel.DateAdded))]
    [MapProperty(nameof(NominatimSearchResultViewModel.Name), nameof(PlaceViewModel.Name))]
    [MapProperty(nameof(NominatimSearchResultViewModel.Address.HouseNumber), nameof(PlaceViewModel.Number))]
    [MapProperty(nameof(NominatimSearchResultViewModel.Address.Postcode), nameof(PlaceViewModel.Postal))]
    [MapProperty(nameof(NominatimSearchResultViewModel.Address.Country), nameof(PlaceViewModel.Country))]
    [MapProperty(nameof(NominatimSearchResultViewModel.Address.CityName), nameof(PlaceViewModel.City))]
    [MapProperty(nameof(NominatimSearchResultViewModel.Address.Road), nameof(PlaceViewModel.Street))]
    public partial PlaceViewModel MapToPlaceViewModel(NominatimSearchResultViewModel nominatimSearchResultViewModel);
}