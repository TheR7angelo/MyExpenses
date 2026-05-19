using System.Collections.ObjectModel;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.ViewModels.Locations;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Presentation.Mappings;

[Mapper(UseDeepCloning = true)]
public partial class LocationDtoViewModelMapper : ILocationDtoViewModelMapper
{
    private static readonly Offset LabelOffset = new() { X = 0, Y = 11 };

    private const string FontFamily = "Arial";

    private const double Size = 12;

    private const double Width = 2;

    public PointFeature MapToPointFeature(PlaceViewModel placeViewModel, ImageStyle? imageStyles = null)
    {
        var point = SphericalMercator.FromLonLat(placeViewModel.Longitude ?? 0, placeViewModel.Latitude ?? 0);
        var pointFeature = MapToPointFeature(point);
        pointFeature[nameof(PlaceViewModel)] = placeViewModel;

        pointFeature.Styles.Clear();
        pointFeature.Styles.Add(new LabelStyle
        {
            Text = placeViewModel.Name, Offset = LabelOffset,
            Font = new Font { FontFamily = FontFamily, Size = Size },
            Halo = new Pen { Color = Color.White, Width = Width }
        });

        if (imageStyles is not null) pointFeature.Styles.Add(imageStyles);

        return pointFeature;
    }

    public TemporaryPointFeature ToTemporaryFeature(PlaceViewModel place, ImageStyle? symbolStyle = null)
    {
        var feature = MapToPointFeature(place, symbolStyle);
        return new TemporaryPointFeature(feature);
    }

    public PlaceViewModel? MapToPlaceViewModel(PointFeature pointFeature)
        => pointFeature[nameof(PlaceViewModel)] as PlaceViewModel;

    public partial PointFeature MapToPointFeature((double x, double y) coordinates);
    public IEnumerable<CountryGroupViewModel> MapToGroup(IEnumerable<PlaceViewModel> placeViewModels)
    {
        // ReSharper disable HeapView.ObjectAllocation.Evident
        var groupedPlacesByCountryCity = placeViewModels
            .GroupBy(country => country.Country)
            .Select(country => new CountryGroupViewModel
            {
                Country = country.Key,
                CityGroups = new ObservableCollection<CityGroupViewModel>(
                    country.GroupBy(s => s.City)
                        .Select(city => new CityGroupViewModel
                        {
                            City = city.Key,
                            Places = new ObservableCollection<PlaceViewModel>(city.ToList())
                        }).ToList())
            }).ToList();
        // ReSharper restore HeapView.ObjectAllocation.Evident

        return groupedPlacesByCountryCity;
    }

    [MapperIgnoreSource(nameof(PlaceViewModel.HasErrors))]
    public partial PlaceDto MapToDto(PlaceViewModel src);

    public partial PlaceViewModel MapToViewModel(PlaceDto src);
}