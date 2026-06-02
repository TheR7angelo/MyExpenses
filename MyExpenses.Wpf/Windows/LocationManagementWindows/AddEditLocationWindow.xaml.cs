using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BruTile.Predefined;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Microsoft.Data.Sqlite;
using MyExpenses.Application.Interfaces;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.WebApi.Nominatim;
using MyExpenses.Presentation;
using MyExpenses.Presentation.Converters;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.RegexUtils;
using MyExpenses.SharedUtils.Resources.Resx.AddEditLocation;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Maps;
using MyExpenses.WebApi.Nominatim;
using Serilog;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using Point = NetTopologySuite.Geometries.Point;
using TemporaryPointFeature = MyExpenses.Models.Mapsui.PointFeatures.TemporaryPointFeature;

namespace MyExpenses.Wpf.Windows.LocationManagementWindows;

public partial class AddEditLocationWindow : IClosable
{
    #region Properties

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EditPlaceProperty = DependencyProperty.Register(nameof(EditPlace),
        typeof(bool), typeof(AddEditLocationWindow), new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EditPlace
    {
        get => (bool)GetValue(EditPlaceProperty);
        set => SetValue(EditPlaceProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TPlace Place { get; } = new();
    public bool PlaceDeleted { get; private set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    private WritableLayer WritableLayer { get; } = new() { Style = null };

    #endregion

    private LocationManagementViewModel ViewModel => (LocationManagementViewModel)DataContext;

    public AddEditLocationWindow(LocationManagementViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    #region Action

    #region Button

    private void ButtonSearchByAddress_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO correct
        // var address = Place.ToString();
        // Log.Information("Using the nominatim API to search via an address : \"{Address}\"", address);
        //
        // var nominatimSearchResults = address.ToNominatim()?.ToList() ?? [];
        // HandleNominatimResult(nominatimSearchResults);
    }

    private void ButtonSearchByCoordinate_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO correct
        // var point = Place.Geometry as Point;
        // Log.Information("Using the nominatim API to search via a point : {Point}", point);
        //
        // var nominatimSearchResult = point?.ToNominatim();
        //
        // var mapper = Mapping.Mapper;
        // var newPlace = mapper.Map<TPlace>(nominatimSearchResult);
        // if (newPlace is null)
        // {
        //     Log.Information("The API returned no result(s)");
        //
        //     Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.ButtonSearchByCoordinateMessageBoxErrorTitle,
        //         AddEditLocationResources.ButtonSearchByCoordinateMessageBoxErrorMessage,
        //         MsgBoxImage.Error);
        //     return;
        // }
        //
        // Log.Information("The API returned one result");
        //
        // newPlace.Id = Place.Id;
        // newPlace.DateAdded = Place.DateAdded ?? newPlace.DateAdded;
        // SetPlace(newPlace, true);
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\"", Place.Name);
        var (success, exception) = Place.Delete();
        if (success)
        {
            Log.Information("Place was successfully removed");
            Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeletePlaceNoUseSuccess, MsgBoxImage.Check);

            PlaceDeleted = true;
            DialogResult = true;

            Close();
            return;
        }

        if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response = Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeletePlaceUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\" with all relative element",
                Place.Name);
            Place.Delete(true);
            Log.Information("Place and all relative element was successfully removed");
            Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeletePlaceUseSuccess, MsgBoxImage.Check);

            PlaceDeleted = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeletePlaceError, MsgBoxImage.Error);
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        string? localizedErrorMessage = null;
        if (string.IsNullOrWhiteSpace(Place.Name)) localizedErrorMessage = AddEditLocationResources.MessageBoxButtonValidationNameError;
        if (string.IsNullOrWhiteSpace(Place.Street)) localizedErrorMessage = AddEditLocationResources.MessageBoxButtonValidationStreetError;
        if (string.IsNullOrWhiteSpace(Place.Postal)) localizedErrorMessage = AddEditLocationResources.MessageBoxButtonValidationPostalError;
        if (string.IsNullOrWhiteSpace(Place.City)) localizedErrorMessage = AddEditLocationResources.MessageBoxButtonValidationCityError;
        if (string.IsNullOrWhiteSpace(Place.Country)) localizedErrorMessage = AddEditLocationResources.MessageBoxButtonValidationCountryError;
        if (Place.Latitude is null or 0) localizedErrorMessage = AddEditLocationResources.MessageBoxButtonValidationLatitudeError;
        if (Place.Longitude is null or 0) localizedErrorMessage = AddEditLocationResources.MessageBoxButtonValidationLongitudeError;
        if (!string.IsNullOrWhiteSpace(localizedErrorMessage))
        {
            Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxButtonValidationTitleError,
                localizedErrorMessage, MessageBoxButton.OK, MsgBoxImage.Error);
            return;
        }

        DialogResult = true;
        Close();
    }

    #endregion

    private void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
    {
        var worldPosition = e.WorldPosition;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var feature = new TemporaryPointFeature(worldPosition)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            Styles = [MapsuiStyleExtensions.GreenMarkerStyle],
            IsTemp = true
        };

        var oldFeature = WritableLayer.GetFeatures().FirstOrDefault(f => (TemporaryPointFeature)f is { IsTemp: true });
        if (oldFeature is not null) WritableLayer.TryRemove(oldFeature);

        WritableLayer.Add(feature);
        MapControl.Map.Refresh();
    }

    #endregion

    #region Function

    private void HandleNominatimResult(IReadOnlyCollection<NominatimSearchResult> nominatimSearchResults)
    {
        // TODO correct
        // TPlace? place = null;
        //
        // switch (nominatimSearchResults.Count)
        // {
        //     case 0:
        //         Log.Information("The API returned no result(s)");
        //         Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxNominatimResultZeroResultTitle,
        //             AddEditLocationResources.MessageBoxNominatimResultZeroResultMessage,
        //             MessageBoxButton.OK, MsgBoxImage.Exclamation);
        //         break;
        //     case 1:
        //         Log.Information("The API returned one result");
        //         Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxNominatimResultOneResultTitle,
        //             AddEditLocationResources.MessageBoxNominatimResultOneResultMessage,
        //             MessageBoxButton.OK, MsgBoxImage.Check);
        //
        //         var nominatimSearchResult = nominatimSearchResults.First();
        //         place = Mapping.Mapper.Map<TPlace>(nominatimSearchResult);
        //         break;
        //     case > 1:
        //         Log.Information("The API returned multiple results ({Count}) :", nominatimSearchResults.Count);
        //         Log.Information("Detailed results: {NominatimSearchResults}", nominatimSearchResults);
        //
        //         Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxNominatimResultMultipleResultTitle,
        //             AddEditLocationResources.MessageBoxNominatimResultMultipleResultMessage,
        //             MessageBoxButton.OK, MsgBoxImage.Information);
        //
        //         var places = nominatimSearchResults
        //             .Select(s => Mapping.Mapper.Map<TPlace>(s));
        //         // ReSharper disable once HeapView.ObjectAllocation.Evident
        //         var nominatimSearchWindows = new NominatimSearchWindow();
        //         nominatimSearchWindows.AddRange(places);
        //         nominatimSearchWindows.ShowDialog();
        //
        //         if (nominatimSearchWindows.DialogResult is not true) return;
        //
        //         place = nominatimSearchWindows.CurrentPlace;
        //         break;
        // }
        //
        // if (place is null) return;
        // SetPlace(place, true);
    }

    public void SetPlace(TPlace newTPlace, bool clear)
    {
        if (clear) WritableLayer.Clear();

        newTPlace.CopyPropertiesTo(Place);
        UpdateMiniMap();
        EditPlace = true;
    }

    public void SetPlace(Point point)
    {
        // TODO correct
        // var nominatim = point.ToNominatim();
        // if (nominatim is not null)
        // {
        //     var mapper = Mapping.Mapper;
        //     var place = mapper.Map<TPlace>(nominatim);
        //     place.CopyPropertiesTo(Place);
        // }
        // else
        // {
        //     Place.Geometry = point;
        // }
        //
        // UpdateMiniMap();
    }

    private void UpdateMiniMap()
    {
        var feature = Place.ToTemporaryFeature(MapsuiStyleExtensions.RedMarkerStyle);
        feature.IsTemp = false;

        WritableLayer.Add(feature);

        MapControl.Map.Navigator.CenterOnAndZoomTo(feature.Point, 1);
        MapControl.Refresh();
    }

    #endregion

    public void LoadPlaceViewModel(PlaceViewModel placeViewModel, bool isEdit)
        => ViewModel.LoadPlaceViewModel(placeViewModel, isEdit);

    private void MapControl_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        var position = Mouse.GetPosition(MapControl);
        ViewModel.OnPositionChanged(position.X, position.Y, MapControl, false);
    }
}