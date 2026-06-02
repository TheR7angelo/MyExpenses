using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BruTile.Predefined;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Groups;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.LocationManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Maps;
using MyExpenses.WebApi.Maps;
using MyExpenses.Wpf.Windows.Dialogs.MsgBox;
using MyExpenses.Wpf.Windows.LocationManagementWindows;
using Serilog;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using Point = NetTopologySuite.Geometries.Point;

namespace MyExpenses.Wpf.Pages;

public partial class LocationManagementPage
{
    public ObservableCollection<CountryGroup> CountryGroups { get; }
    public List<KnownTileSource> KnownTileSources { get; }
    public KnownTileSource KnownTileSourceSelected { get; set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // The PlaceLayer instance is used to store the features of the places.
    private WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(TPlace) };
    private IEnumerable<ILayer> InfoLayers { get; }

    private TPlace? ClickTPlace { get; set; }
    private Point ClickPoint { get; set; } = Point.Empty;
    private PointFeature? PointFeature { get; set; }

    private LocationManagementViewModel LocationManagementViewModel
        => (LocationManagementViewModel)DataContext;

    public LocationManagementPage(LocationManagementViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
    }

    #region Action

    private void MapControl_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        var position = Mouse.GetPosition(MapControl);
        LocationManagementViewModel.OnPositionChanged(position.X, position.Y, MapControl, true);
    }

    private void MenuItemDeleteFeature_OnClick(object sender, RoutedEventArgs e)
    {
        // var feature = PointFeature;
        // if (feature is null) return;
        //
        // var placeToDelete = feature.ToTPlace();
        // var message = string.Format(LocationManagementResources.MessageBoxDeleteQuestionMessage, placeToDelete.Name);
        // var response =
        //     MsgBox.Show(LocationManagementResources.MessageBoxDeleteQuestionTitle, message,
        //         MessageBoxButton.YesNoCancel, MsgBoxImage.Question);
        // if (response is not MessageBoxResult.Yes) return;
        //
        // Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\"", placeToDelete.Name);
        // var (success, exception) = placeToDelete.Delete();
        //
        // if (success)
        // {
        //     PlaceLayer.TryRemove(feature);
        //     MapControl.Refresh();
        //
        //     Log.Information("Place was successfully removed");
        //     MsgBox.Show(LocationManagementResources.MessageBoxMenuItemDeleteFeatureNoUseSuccessTitle,
        //         LocationManagementResources.MessageBoxMenuItemDeleteFeatureNoUseSuccessMessage,
        //         MsgBoxImage.Check);
        // }
        // else if (exception!.InnerException is SqliteException
        //     {
        //         SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
        //     })
        // {
        //     Log.Error("Foreign key constraint violation");
        //
        //     response =
        //         MsgBox.Show(LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseQuestionTitle,
        //             LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseQuestionMessage,
        //             MessageBoxButton.YesNoCancel, MsgBoxImage.Question);
        //
        //     if (response is not MessageBoxResult.Yes) return;
        //
        //     Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\" with all relative element",
        //         placeToDelete.Name);
        //     placeToDelete.Delete(true);
        //     Log.Information("Place and all relative element was successfully removed");
        //     MsgBox.Show(LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseSuccessTitle,
        //         LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseSuccessMessage, MsgBoxImage.Check);
        //
        //     RemovePlaceTreeViewCountryGroup(placeToDelete);
        //
        //     MapControl.Refresh();
        //
        //     return;
        // }
        //
        // Log.Error(exception, "An error occurred please retry");
        // MsgBox.Show(LocationManagementResources.MessageBoxMenuItemDeleteFeatureErrorTitle,
        //     LocationManagementResources.MessageBoxMenuItemDeleteFeatureErrorMessage,
        //     MsgBoxImage.Error);
    }

    private void MenuItemEditFeature_OnClick(object sender, RoutedEventArgs e)
    {
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // The AddEditLocationWindow instance is created to manage the addition or editing of a location.
        // // The SetPlace method is called with the current ClickTPlace and an additional parameter to configure the dialog appropriately.
        // // ShowDialog() is used to display the window modally, halting execution until the user closes the dialog.
        // var addEditLocationWindow = new AddEditLocationWindow();
        // addEditLocationWindow.SetPlace(ClickTPlace!, false);
        // addEditLocationWindow.ShowDialog();
        //
        // if (addEditLocationWindow.DialogResult is not true) return;
        //
        // var editedPlace = addEditLocationWindow.Place;
        // var success = ProcessNewPlace(editedPlace, edit: true);
        // if (!success) return;
        //
        // RemovePlaceTreeViewCountryGroup(editedPlace);
        // AddPlaceTreeViewCountryGroup(editedPlace);
    }

    private void MenuItemToGoogleEarthWeb_OnClick(object sender, RoutedEventArgs e)
    {
        // var log = ClickTPlace.GetLogForGoogleEarthWeb(ClickPoint);
        //
        // Log.Information("{Log}", log);
        // var uri = ClickPoint.ToGoogleEarthWeb(ProjectSystem.Wpf);
        // Log.Information("{Uri}", uri);
    }


    private void MenuItemToGoogleMaps_OnClick(object sender, RoutedEventArgs e)
    {
        // var log = ClickTPlace.GetLogForGoogleMaps(ClickPoint);
        //
        // Log.Information("{Log}", log);
        // var uri = ClickPoint.ToGoogleMaps(ProjectSystem.Wpf);
        // Log.Information("{Uri}", uri);
    }

    private void MenuItemToGoogleStreetView_OnClick(object sender, RoutedEventArgs e)
    {
        // var log = ClickTPlace.GetLogForGoogleStreetView(ClickPoint);
        //
        // Log.Information("{Log}", log);
        // var uri = ClickPoint.ToGoogleStreetView(ProjectSystem.Wpf);
        // Log.Information("{Uri}", uri);
    }

    #endregion

    #region Function

    // ReSharper disable once HeapView.ClosureAllocation
    // private void AddPlaceTreeViewCountryGroup(TPlace placeToAdd)
    // {
    //     // ReSharper disable HeapView.DelegateAllocation
    //     var cityGroup = CountryGroups.FirstOrDefault(s => s.Country == placeToAdd.Country)?.CityGroups
    //         ?.FirstOrDefault(s => s.City == placeToAdd.City);
    //     // ReSharper restore HeapView.DelegateAllocation
    //
    //     if (cityGroup is null)
    //     {
    //         // ReSharper disable once HeapView.ObjectAllocation.Evident
    //         // The newCityGroup instance is used to store the information of the city.
    //         var newCityGroup = new CityGroup { City = placeToAdd.City, Places = [placeToAdd] };
    //
    //         // ReSharper disable once HeapView.DelegateAllocation
    //         var countryGroup = CountryGroups.FirstOrDefault(s => s.Country == placeToAdd.Country);
    //         if (countryGroup is null)
    //         {
    //             // ReSharper disable once HeapView.ObjectAllocation.Evident
    //             // The newGroupCountry instance is used to store the information of the country.
    //             var newGroupCountry = new CountryGroup { Country = placeToAdd.Country, CityGroups = [newCityGroup] };
    //             CountryGroups.AddAndSort(newGroupCountry, s => s.Country ?? string.Empty);
    //         }
    //         else
    //         {
    //             countryGroup.CityGroups?.AddAndSort(newCityGroup, s => s.City ?? string.Empty);
    //         }
    //     }
    //     else
    //     {
    //         cityGroup.Places?.AddAndSort(placeToAdd, s => s.Name ?? string.Empty);
    //     }
    // }

    // private bool ProcessNewPlace(TPlace newPlace, bool add = false, bool edit = false)
    // {
    //     var (success, exception) = newPlace.AddOrEdit();
    //     if (success)
    //     {
    //         var feature = newPlace.IsOpen
    //             ? newPlace.ToFeature(MapsuiStyleExtensions.RedMarkerStyle)
    //             : newPlace.ToFeature(MapsuiStyleExtensions.BlueMarkerStyle);
    //
    //         PlaceLayer.TryRemove(PointFeature!);
    //         PlaceLayer.Add(feature);
    //         MapControl.Refresh();
    //
    //         // string json;
    //         switch (add)
    //         {
    //             case true when !edit:
    //                 MsgBox.Show(LocationManagementResources.MessageBoxProcessNewPlaceAddSuccess, MsgBoxImage.Check);
    //
    //                 Log.Information("The new place was successfully added");
    //
    //                 // Loop crash
    //                 // json = newPlace.ToJsonString();
    //                 // Log.Information("{Json}", json);
    //
    //                 break;
    //             case false when edit:
    //                 MsgBox.Show(LocationManagementResources.MessageBoxProcessNewPlaceEditSuccess,
    //                     MsgBoxImage.Check);
    //
    //                 Log.Information("The new place was successfully edited");
    //
    //                 // Loop crash
    //                 // json = newPlace.ToJsonString();
    //                 // Log.Information("{Json}", json);
    //
    //                 break;
    //         }
    //
    //         return true;
    //     }
    //
    //     Log.Error(exception, "An error occurred please retry");
    //     MsgBox.Show(LocationManagementResources.MessageBoxProcessNewPlaceError, MsgBoxImage.Error);
    //
    //     return false;
    // }

    // // ReSharper disable once HeapView.ClosureAllocation
    // private void RemovePlaceTreeViewCountryGroup(TPlace placeToDelete)
    // {
    //     var countryToRemove = CountryGroups
    //         .FirstOrDefault(countryGroup => countryGroup.CityGroups is not null &&
    //                                         countryGroup.CityGroups.Any(cityGroup => cityGroup.Places is not null &&
    //                                             // ReSharper disable once HeapView.DelegateAllocation
    //                                             cityGroup.Places.Any(place => place.Id == placeToDelete.Id)));
    //
    //     var cityToRemove = countryToRemove?
    //         .CityGroups?.FirstOrDefault(cityGroup => cityGroup.Places is not null &&
    //                                                  // ReSharper disable once HeapView.DelegateAllocation
    //                                                  cityGroup.Places.Any(place => place.Id == placeToDelete.Id));
    //
    //     var placeToRemove = cityToRemove?.Places?
    //         // ReSharper disable once HeapView.DelegateAllocation
    //         .FirstOrDefault(place => place.Id.Equals(placeToDelete.Id));
    //     if (placeToRemove is null) return;
    //
    //     cityToRemove?.Places?.Remove(placeToRemove);
    //
    //     if (cityToRemove?.Places?.Count == 0) countryToRemove?.CityGroups?.Remove(cityToRemove);
    //
    //     if (countryToRemove?.CityGroups?.Count == 0) CountryGroups.Remove(countryToRemove);
    // }

    // private void SetZoom(params MPoint[] points)
    //     => MapControl.Map.Navigator.SetZoom(points);

    // private void UpdateMapBackColor()
    // {
    //     var backColor = Utils.Resources.GetMaterialDesignPaperMapsUiStylesColor();
    //     MapControl.Map.BackColor = backColor;
    // }

    // private void UpdateTileLayer()
    // {
    //     const string layerName = "Background";
    //
    //     var httpTileSource = BruTile.Predefined.KnownTileSources.Create(KnownTileSourceSelected);
    //
    //     // ReSharper disable once HeapView.ObjectAllocation.Evident
    //     // A new instance of TileLayer is created here using the specified httpTileSource.
    //     // This layer is responsible for rendering map tiles from the configured tile source,
    //     // allowing the application to display background maps or other geographic data dynamically
    //     // based on the selected tile provider.
    //     var tileLayer = new TileLayer(httpTileSource);
    //     tileLayer.Name = layerName;
    //
    //     var layers = MapControl?.Map.Layers.FindLayer(layerName);
    //     if (layers is not null) MapControl?.Map.Layers.Remove(layers.ToArray());
    //
    //     MapControl?.Map.Layers.Insert(0, tileLayer);
    // }

    #endregion

    // private void Option1_Click(object sender, RoutedEventArgs e)
    // {
    //     Console.WriteLine(ClickPoint);
    // }
    //
    // private void Option2_Click(object sender, RoutedEventArgs e)
    // {
    //     var s = ClickPoint.ToNominatim();
    //     Console.WriteLine(s);
    // }
}