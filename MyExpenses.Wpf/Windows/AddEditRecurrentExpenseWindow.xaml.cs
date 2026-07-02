using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mapsui.Layers;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Presentation;
using MyExpenses.Presentation.Converters;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Maps;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Converters;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditRecurrentExpenseWindow;
using Serilog;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditRecurrentExpenseWindow
{
    #region DependencyProperty

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EditRecurrentExpenseProperty =
        DependencyProperty.Register(nameof(EditRecurrentExpense), typeof(bool), typeof(AddEditRecurrentExpenseWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EditRecurrentExpense
    {
        get => (bool)GetValue(EditRecurrentExpenseProperty);
        set => SetValue(EditRecurrentExpenseProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty SelectedCountryProperty =
        DependencyProperty.Register(nameof(SelectedCountry), typeof(string), typeof(AddEditRecurrentExpenseWindow),
            new PropertyMetadata(default(string)));

    public string? SelectedCountry
    {
        get => (string)GetValue(SelectedCountryProperty);
        set => SetValue(SelectedCountryProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty SelectedCityProperty = DependencyProperty.Register(nameof(SelectedCity),
        typeof(string), typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string SelectedCity
    {
        get => (string)GetValue(SelectedCityProperty);
        set => SetValue(SelectedCityProperty, value);
    }

    #endregion

    #region Property

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    private WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(TPlace) };

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TRecursiveExpense RecursiveExpense { get; } = new();

    #endregion

    private RecurringExpenseManagementViewModel RecurringExpenseManagementViewModel
        => (RecurringExpenseManagementViewModel)DataContext;

    public AddEditRecurrentExpenseWindow(RecurringExpenseManagementViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = Dialogs.MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the recursive expense \"{RecursiveExpenseDescription}\"", RecursiveExpense.Description);
        var (success, exception) = RecursiveExpense.Delete();

        if (success)
        {
            Log.Information("Recursive expense was successfully removed");
            Dialogs.MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxDeleteRecursiveExpenseNoUseSuccess,
                MsgBoxImage.Check);

            // RecursiveExpenseDeleted = true;
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

            response = Dialogs.MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxDeleteRecursiveExpenseUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information("Attempting to remove the recursive expense \"{RecursiveExpenseDescription}\" with all relative element",
                RecursiveExpense.Description);
            RecursiveExpense.Delete(true);
            Log.Information("Recursive expense and all relative element was successfully removed");
            Dialogs.MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxDeleteRecursiveExpenseUseSuccess,
                MsgBoxImage.Check);

            // RecursiveExpenseDeleted = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        Dialogs.MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxDeleteRecursiveExpenseError, MsgBoxImage.Error);
    }

    private void ButtonPlace_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO correct
        // var place = RecursiveExpense.PlaceFk?.ToISql<TPlace>();
        // if (place?.CanBeDeleted is false)
        // {
        //     Dialogs.MsgBox.MsgBox.Show(LocationManagementResources.MessageBoxPlaceCantEditMessage, MsgBoxImage.Error);
        //     return;
        // }
        //
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // var addEditLocationWindow = new AddEditLocationWindow();
        // if (place is not null) addEditLocationWindow.SetPlace(place, false);
        //
        // var result = addEditLocationWindow.ShowDialog();
        // if (result is not true) return;
        //
        // // ReSharper disable once HeapView.DelegateAllocation
        // var oldPlace = PlacesCollection.FirstOrDefault(s => s.Id == RecursiveExpense.PlaceFk);
        // if (addEditLocationWindow.PlaceDeleted)
        // {
        //     if (oldPlace is not null) PlacesCollection.Remove(oldPlace);
        //
        //     return;
        // }
        //
        // var editedPlace = addEditLocationWindow.Place;
        // Log.Information("Attempting to update place id:\"{EditedPlaceId}\", name:\"{EditedPlaceName}\"", editedPlace.Id,
        //     editedPlace.Name);
        //
        // var (success, exception) = editedPlace.AddOrEdit();
        // if (success)
        // {
        //     PlacesCollection!.AddAndSort(oldPlace, editedPlace, s => s!.Name!);
        //     RecursiveExpense.PlaceFk = editedPlace.Id;
        //
        //     Log.Information("Place was successfully edited");
        //
        //     // Loop crash
        //     // var json = editedPlace.ToJsonString();
        //     // Log.Information("{Json}", json);
        //
        //     Dialogs.MsgBox.MsgBox.Show(LocationManagementResources.MessageBoxEditPlaceSuccessMessage, MsgBoxImage.Check);
        // }
        // else
        // {
        //     Log.Error(exception, "An error occurred please retry");
        //     Dialogs.MsgBox.MsgBox.Show(LocationManagementResources.MessageBoxEditPlaceErrorMessage, MsgBoxImage.Error);
        // }
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The serviceProvider and items are set to null because they are not required in this context.
        // The ValidationResults list will store any validation errors detected during the process.
        var validationContext = new ValidationContext(RecursiveExpense, serviceProvider: null, items: null);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Using 'var' keeps the code concise and readable, as the type (List<ValidationResult>)
        // is evident from the initialization. The result will still be compatible with any method
        // that expects an ICollection<ValidationResult>, as List<T> implements the ICollection interface.
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(RecursiveExpense, validationContext, validationResults, true);

        if (!isValid)
        {
            var propertyError = validationResults.First();
            var propertyMemberName = propertyError.MemberNames.First();

            var messageErrorKey = propertyMemberName switch
            {
                nameof(TRecursiveExpense.AccountFk) => nameof(AddEditRecurrentExpenseWindowResources
                    .MessageBoxButtonValidationAccountFkError),
                nameof(TRecursiveExpense.Description) => nameof(AddEditRecurrentExpenseWindowResources
                    .MessageBoxButtonValidationDescriptionError),
                nameof(TRecursiveExpense.CategoryTypeFk) => nameof(AddEditRecurrentExpenseWindowResources
                    .MessageBoxButtonValidationCategoryTypeFkError),
                nameof(TRecursiveExpense.ModePaymentFk) => nameof(AddEditRecurrentExpenseWindowResources
                    .MessageBoxButtonValidationModePaymentFkError),
                nameof(TRecursiveExpense.Value) => nameof(AddEditRecurrentExpenseWindowResources
                    .MessageBoxButtonValidationValueError),
                nameof(TRecursiveExpense.PlaceFk) => nameof(AddEditRecurrentExpenseWindowResources
                    .MessageBoxButtonValidationPlaceFkError),
                nameof(TRecursiveExpense.StartDate) => nameof(AddEditRecurrentExpenseWindowResources
                    .MessageBoxButtonValidationStartDateError),
                nameof(TRecursiveExpense.FrequencyFk) => nameof(AddEditRecurrentExpenseWindowResources
                    .MessageBoxButtonValidationFrequencyFkError),
                _ => null
            };

            var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
                ? propertyError.ErrorMessage!
                : AddEditRecurrentExpenseWindowResources.ResourceManager.GetString(messageErrorKey)!;

            Dialogs.MsgBox.MsgBox.Show(localizedErrorMessage, MsgBoxImage.Error);
            return;
        }

        Log.Information("Attempting to inject the new recursive expense");

        RecursiveExpense.LastUpdated = DateTime.Now;
        var (success, exception) = RecursiveExpense.AddOrEdit();
        if (success)
        {
            Log.Information("Recursive expense was successfully added");
            var json = RecursiveExpense.ToJsonString();
            Log.Information("{Json}", json);

            Dialogs.MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxAddRecursiveExpenseSuccess, MsgBoxImage.Check);

            if (EditRecurrentExpense)
            {
                DialogResult = true;
                Close();
                return;
            }

            var response = Dialogs.MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxAddRecursiveExpenseQuestion, MsgBoxImage.Question,
                MessageBoxButton.YesNoCancel);
            if (response is not MessageBoxResult.Yes) Close();

            RecursiveExpense.Reset();
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            Dialogs.MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxAddRecursiveExpenseError, MsgBoxImage.Error);
        }
    }

    #endregion

    #region Function

    public void SetVRecursiveExpense(VRecursiveExpense vRecurrentExpense)
    {
        var place = vRecurrentExpense.PlaceFk?.ToISql<TPlace>();
        SelectedCountry = EmptyStringTreeViewConverter.ToUnknown(place?.Country);
        SelectedCity = EmptyStringTreeViewConverter.ToUnknown(place?.City);

        EditRecurrentExpense = true;
        vRecurrentExpense.CopyPropertiesTo(RecursiveExpense);
    }

    private void UpdateMapPoint(TPlace? place)
    {
        PlaceLayer.Clear();

        if (place is null)
        {
            MapControl.Refresh();
            return;
        }

        var pointFeature = place.ToFeature(MapsuiStyleExtensions.RedMarkerStyle);

        PlaceLayer.Add(pointFeature);
        MapControl.Map.Navigator.CenterOnAndZoomTo(pointFeature.Point);
    }

    #endregion

    private void MapControl_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        var position = Mouse.GetPosition(MapControl);
        RecurringExpenseManagementViewModel.LocationManagementViewModel.OnPositionChanged(position.X, position.Y, MapControl, true);
    }
}