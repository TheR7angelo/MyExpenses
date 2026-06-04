using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Application.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.SharedUtils.Resources.Resx.AddEditLocation;
using MyExpenses.Sql.Context;
using Serilog;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace MyExpenses.Wpf.Windows.LocationManagementWindows;

public partial class AddEditLocationWindow : IClosable
{
    #region Properties

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TPlace Place { get; } = new();
    public bool PlaceDeleted { get; private set; }

    #endregion

    private LocationManagementViewModel ViewModel => (LocationManagementViewModel)DataContext;

    public AddEditLocationWindow(LocationManagementViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    #region Action

    #region Button

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        // var response = Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeleteQuestion,
        //     MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        // if (response is not MessageBoxResult.Yes) return;
        //
        // Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\"", Place.Name);
        // var (success, exception) = Place.Delete();
        // if (success)
        // {
        //     Log.Information("Place was successfully removed");
        //     Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeletePlaceNoUseSuccess, MsgBoxImage.Check);
        //
        //     PlaceDeleted = true;
        //     DialogResult = true;
        //
        //     Close();
        //     return;
        // }
        //
        // if (exception!.InnerException is SqliteException
        //     {
        //         SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
        //     })
        // {
        //     Log.Error("Foreign key constraint violation");
        //
        //     response = Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeletePlaceUseQuestion,
        //         MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        //
        //     if (response is not MessageBoxResult.Yes) return;
        //
        //     Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\" with all relative element",
        //         Place.Name);
        //     Place.Delete(true);
        //     Log.Information("Place and all relative element was successfully removed");
        //     Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeletePlaceUseSuccess, MsgBoxImage.Check);
        //
        //     PlaceDeleted = true;
        //     DialogResult = true;
        //     Close();
        //
        //     return;
        // }
        //
        // Log.Error(exception, "An error occurred please retry");
        // Dialogs.MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeletePlaceError, MsgBoxImage.Error);
    }

    #endregion

    #endregion

    public void LoadPlaceViewModel(PlaceViewModel placeViewModel, bool isEdit)
        => ViewModel.LoadPlaceViewModel(placeViewModel, isEdit);

    private void MapControl_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        var position = Mouse.GetPosition(MapControl);
        ViewModel.OnPositionChanged(position.X, position.Y, MapControl, false);
    }
}