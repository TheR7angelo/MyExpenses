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
}