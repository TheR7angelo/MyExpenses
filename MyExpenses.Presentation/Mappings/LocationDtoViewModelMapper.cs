using System.Collections.ObjectModel;
using Mapsui;
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

    public MRect MapToMRect(MPoint[] points, double margin = 10d)
    {
        double minX = points.Min(p => p.X), maxX = points.Max(p => p.X);
        double minY = points.Min(p => p.Y), maxY = points.Max(p => p.Y);

        var width = maxX - minX;
        var height = maxY - minY;

        var marginX = width * margin / 100;
        var marginY = height * margin / 100;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The mRect instance is used to store the coordinates of the rectangle to zoom to.
        return new MRect(minX - marginX, minY - marginY, maxX + marginX, maxY + marginY);
    }

    public MRect MapToMRect(IEnumerable<MPoint> points, double margin = 10d)
        => MapToMRect(points.ToArray(), margin);

    public MPoint MapToMPoint(PlaceViewModel place)
    {
        var pointFeature = MapToPointFeature(place);
        return pointFeature.Point;
    }

    [MapperIgnoreSource(nameof(PlaceViewModel.HasErrors))]
    public partial PlaceDto MapToDto(PlaceViewModel src);

    public partial PlaceViewModel MapToViewModel(PlaceDto src);
}