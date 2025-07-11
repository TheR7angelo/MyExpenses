using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Objects;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.AddEditAccount;
using MyExpenses.SharedUtils.Resources.Resx.CategoryTypesManagement;
using MyExpenses.SharedUtils.Resources.Resx.DetailedRecordManagement;
using MyExpenses.SharedUtils.Resources.Resx.LocationManagement;
using MyExpenses.SharedUtils.Resources.Resx.ModePaymentManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Maps;
using MyExpenses.Utils.Resources.Resx.Converters.EmptyStringTreeViewConverter;
using MyExpenses.Wpf.Windows;
using MyExpenses.Utils.Sql;
using MyExpenses.Utils.Strings;
using MyExpenses.Wpf.Converters;
// using MyExpenses.Wpf.Resources.Resx.Pages.RecordExpensePage;
using MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;
using MyExpenses.Wpf.Windows.LocationManagementWindows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace MyExpenses.Wpf.Pages;

public partial class RecordExpensePage
{

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EditHistoryProperty = DependencyProperty.Register(nameof(EditHistory),
        typeof(bool), typeof(RecordExpensePage), new PropertyMetadata(false));

    public bool EditHistory
    {
        get => (bool)GetValue(EditHistoryProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(EditHistoryProperty, value);
    }

    public static readonly DependencyProperty SelectedCountryProperty =
        DependencyProperty.Register(nameof(SelectedCountry), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string? SelectedCountry
    {
        get => (string)GetValue(SelectedCountryProperty);
        set => SetValue(SelectedCountryProperty, value);
    }

    public static readonly DependencyProperty SelectedCityProperty = DependencyProperty.Register(nameof(SelectedCity),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(string), typeof(RecordExpensePage), new PropertyMetadata(default(string)));

    public string SelectedCity
    {
        get => (string)GetValue(SelectedCityProperty);
        set => SetValue(SelectedCityProperty, value);
    }

    public static readonly DependencyProperty ComboBoxAccountHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxAccountHintAssist), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxAccountHintAssist
    {
        get => (string)GetValue(ComboBoxAccountHintAssistProperty);
        set => SetValue(ComboBoxAccountHintAssistProperty, value);
    }

    public static readonly DependencyProperty TextBoxDescriptionHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxDescriptionHintAssist), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string TextBoxDescriptionHintAssist
    {
        get => (string)GetValue(TextBoxDescriptionHintAssistProperty);
        set => SetValue(TextBoxDescriptionHintAssistProperty, value);
    }

    public static readonly DependencyProperty ComboBoxCategoryTypeHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxCategoryTypeHintAssist), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxCategoryTypeHintAssist
    {
        get => (string)GetValue(ComboBoxCategoryTypeHintAssistProperty);
        set => SetValue(ComboBoxCategoryTypeHintAssistProperty, value);
    }

    public static readonly DependencyProperty ComboBoxModePaymentHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxModePaymentHintAssist), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxModePaymentHintAssist
    {
        get => (string)GetValue(ComboBoxModePaymentHintAssistProperty);
        set => SetValue(ComboBoxModePaymentHintAssistProperty, value);
    }

    public static readonly DependencyProperty TextBoxValueHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxValueHintAssist), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string TextBoxValueHintAssist
    {
        get => (string)GetValue(TextBoxValueHintAssistProperty);
        set => SetValue(TextBoxValueHintAssistProperty, value);
    }

    public static readonly DependencyProperty DatePickerWhenHintAssistProperty =
        DependencyProperty.Register(nameof(DatePickerWhenHintAssist), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string DatePickerWhenHintAssist
    {
        get => (string)GetValue(DatePickerWhenHintAssistProperty);
        set => SetValue(DatePickerWhenHintAssistProperty, value);
    }

    public static readonly DependencyProperty TimePickerWhenHintAssistProperty =
        DependencyProperty.Register(nameof(TimePickerWhenHintAssist), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string TimePickerWhenHintAssist
    {
        get => (string)GetValue(TimePickerWhenHintAssistProperty);
        set => SetValue(TimePickerWhenHintAssistProperty, value);
    }

    public static readonly DependencyProperty ComboBoxPlaceHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxPlaceHintAssist), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxPlaceHintAssist
    {
        get => (string)GetValue(ComboBoxPlaceHintAssistProperty);
        set => SetValue(ComboBoxPlaceHintAssistProperty, value);
    }

    public static readonly DependencyProperty CheckBoxPointedContentProperty =
        DependencyProperty.Register(nameof(CheckBoxPointedContent), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string CheckBoxPointedContent
    {
        get => (string)GetValue(CheckBoxPointedContentProperty);
        set => SetValue(CheckBoxPointedContentProperty, value);
    }

    public static readonly DependencyProperty ButtonValidContentProperty =
        DependencyProperty.Register(nameof(ButtonValidContent), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    public static readonly DependencyProperty ButtonDeleteContentProperty =
        DependencyProperty.Register(nameof(ButtonDeleteContent), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonDeleteContent
    {
        get => (string)GetValue(ButtonDeleteContentProperty);
        set => SetValue(ButtonDeleteContentProperty, value);
    }

    public static readonly DependencyProperty ComboBoxBackgroundHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxBackgroundHintAssist), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxBackgroundHintAssist
    {
        get => (string)GetValue(ComboBoxBackgroundHintAssistProperty);
        set => SetValue(ComboBoxBackgroundHintAssistProperty, value);
    }

    public static readonly DependencyProperty ComboBoxPlaceCountryHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxPlaceCountryHintAssist), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxPlaceCountryHintAssist
    {
        get => (string)GetValue(ComboBoxPlaceCountryHintAssistProperty);
        set => SetValue(ComboBoxPlaceCountryHintAssistProperty, value);
    }

    public static readonly DependencyProperty ComboBoxPlaceCityHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxPlaceCityHintAssist), typeof(string), typeof(RecordExpensePage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxPlaceCityHintAssist
    {
        get => (string)GetValue(ComboBoxPlaceCityHintAssistProperty);
        set => SetValue(ComboBoxPlaceCityHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public THistory History { get; } = new();

    public string SelectedValuePathAccount { get; } = nameof(TAccount.Id);
    public string DisplayMemberPathAccount { get; } = nameof(TAccount.Name);
    public string SelectedValuePathCategoryType { get; } = nameof(TCategoryType.Id);
    public string DisplayMemberPathCategoryType { get; } = nameof(TCategoryType.Name);
    public string SelectedValuePathModePayment { get; } = nameof(TModePayment.Id);
    public string DisplayMemberPathModePayment { get; } = nameof(TModePayment.Name);
    public string SelectedValuePathPlace { get; } = nameof(TPlace.Id);
    public string DisplayMemberPathPlaceName { get; } = nameof(TPlace.Name);

    public ObservableCollection<TAccount> Accounts { get; }
    public ObservableCollection<TCategoryType> CategoryTypes { get; }
    public ObservableCollection<TModePayment> ModePayments { get; }

    public ObservableCollection<string> CountriesCollection { get; }

    public ObservableCollection<string> CitiesCollection { get; }
    public ObservableCollection<TPlace> PlacesCollection { get; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // A new WritableLayer instance is intentionally allocated here to represent the layer
    // dedicated to places (TPlace). This layer acts as a container for displaying map features
    // related to places and provides the flexibility to dynamically add or remove features
    // as needed. By creating a unique instance for each `DetailedRecordContentPage`, we
    // ensure that map layers remain properly isolated and don't interfere with layers
    // managed by other pages or components in the application.
    private WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(TPlace) };
    public List<KnownTileSource> KnownTileSources { get; }
    public KnownTileSource KnownTileSourceSelected { get; set; }

    public RecordExpensePage()
    {
        KnownTileSources = [..MapsuiMapExtensions.GetAllKnowTileSource()];

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];
        CategoryTypes = [..context.TCategoryTypes.OrderBy(s => s.Name)];
        ModePayments = [..context.TModePayments.OrderBy(s => s.Name)];

        PlacesCollection = [..context.TPlaces.Where(s => s.IsOpen).OrderBy(s => s.Name)];

        var records = PlacesCollection.Select(s => EmptyStringTreeViewConverter.ToUnknown(s.Country)).Order()
            .Distinct();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        CountriesCollection = new ObservableCollection<string>(records);

        records = PlacesCollection.Select(s => EmptyStringTreeViewConverter.ToUnknown(s.City)).Order().Distinct();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        CitiesCollection = new ObservableCollection<string>(records);

        var backColor = Utils.Resources.GetMaterialDesignPaperMapsUiStylesColor();
        var map = MapsuiMapExtensions.GetMap(true, backColor);
        map.Layers.Add(PlaceLayer);

        InitializeComponent();

        History.Date = DateTime.Now;

        UpdateConfiguration();

        MapControl.Map = map;

        // ReSharper disable once HeapView.DelegateAllocation
        Configuration.ConfigurationChanged += Configuration_OnConfigurationChanged;
    }

    #region Action

    private void ButtonAccount_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of AddEditAccountWindow is created to handle adding or editing an account.
        var addEditAccountWindow = new AddEditAccountWindow();

        var account = History.AccountFk?.ToISql<TAccount>();
        if (account is not null) addEditAccountWindow.SetTAccount(account);

        addEditAccountWindow.ShowDialog();
        if (addEditAccountWindow.DialogResult is not true) return;

        if (addEditAccountWindow.DeleteAccount)
        {
            // ReSharper disable once HeapView.DelegateAllocation
            var accountToRemove = Accounts.FirstOrDefault(s => s.Id == History.AccountFk);
            if (accountToRemove is not null) Accounts.Remove(accountToRemove);
        }
        else
        {
            var editedAccount = addEditAccountWindow.Account;

            Log.Information("Attempting to edit the account \"{AccountName}\"", editedAccount.Name);
            var (success, exception) = editedAccount.AddOrEdit();
            if (success)
            {
                Log.Information("Account was successfully edited");
                var json = editedAccount.ToJsonString();
                Log.Information("{Json}", json);

                MsgBox.Show(AddEditAccountResources.MessageBoxEditAccountSuccessMessage, MsgBoxImage.Check);

                // ReSharper disable once HeapView.DelegateAllocation
                var accountToRemove = Accounts.FirstOrDefault(s => s.Id == History.AccountFk);
                Accounts!.AddAndSort(accountToRemove, editedAccount, s => s?.Name!);

                History.AccountFk = editedAccount.Id;
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MsgBox.Show(AddEditAccountResources.MessageBoxEditAccountErrorMessage, MsgBoxImage.Warning);
            }
        }
    }

    private void ButtonCategoryType_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of AddEditCategoryTypeWindow is created to handle adding or editing a category type.
        // The category type is set to null if the category type is not found.
        var addEditCategoryTypeWindow = new AddEditCategoryTypeWindow();
        var categoryType = History.CategoryTypeFk?.ToISql<TCategoryType>();
        if (categoryType is not null) addEditCategoryTypeWindow.SetTCategoryType(categoryType);

        var result = addEditCategoryTypeWindow.ShowDialog();
        if (result is not true) return;

        if (addEditCategoryTypeWindow.CategoryTypeDeleted)
        {
            // ReSharper disable once HeapView.DelegateAllocation
            var categoryTypeToRemove = CategoryTypes.FirstOrDefault(s => s.Id == History.CategoryTypeFk);
            if (categoryTypeToRemove is not null) CategoryTypes.Remove(categoryTypeToRemove);
        }
        else
        {
            var editedCategoryType = addEditCategoryTypeWindow.CategoryType;
            Log.Information("Attempting to edit the category type id: {Id}", editedCategoryType.Id);

            // ReSharper disable once HeapView.ClosureAllocation
            var editedCategoryTypeDeepCopy = editedCategoryType.DeepCopy()!;

            var (success, exception) = editedCategoryType.AddOrEdit();
            if (success)
            {
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                // Necessary instantiation of DataBaseContext to interact with the database.
                // This creates a scoped database context for performing queries and modifications in the database.
                using var context = new DataBaseContext();
                editedCategoryTypeDeepCopy.ColorFkNavigation =
                    context.TColors.FirstOrDefault(s => s.Id == editedCategoryTypeDeepCopy.ColorFk);

                CategoryTypes!.AddAndSort(categoryType, editedCategoryTypeDeepCopy, s => s!.Name!);

                Log.Information("Category type was successfully edited");
                var json = editedCategoryTypeDeepCopy.ToJsonString();
                Log.Information("{Json}", json);

                MsgBox.Show(CategoryTypesManagementResources.MessageBoxCategoryTypeEditSuccessTitle,
                    CategoryTypesManagementResources.MessageBoxCategoryTypeEditSuccessMessage, MsgBoxImage.Check);
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MsgBox.Show(CategoryTypesManagementResources.MessageBoxCategoryTypeEditErrorTitle,
                    CategoryTypesManagementResources.MessageBoxCategoryTypeEditErrorMessage, MsgBoxImage.Error);
            }
        }
    }

    private void ButtonModePayment_OnClick(object sender, RoutedEventArgs e)
    {
        var modePayment = History.ModePaymentFk?.ToISql<TModePayment>();
        if (modePayment?.CanBeDeleted is false)
        {
            MsgBox.Show(ModePaymentManagementResources.MessageBoxModePaymentCantEditMessage, MsgBoxImage.Error);
            return;
        }

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of AddEditModePaymentWindow is created to handle adding or editing a mode payment.
        // The mode payment is set to null if the mode payment is not found.
        var addEditModePaymentWindow = new AddEditModePaymentWindow();
        if (modePayment is not null) addEditModePaymentWindow.SetTModePayment(modePayment);

        var result = addEditModePaymentWindow.ShowDialog();
        if (result is not true) return;

        // ReSharper disable once HeapView.DelegateAllocation
        var modePaymentToRemove = ModePayments.FirstOrDefault(s => s.Id == History.ModePaymentFk);
        if (addEditModePaymentWindow.ModePaymentDeleted)
        {
            if (modePaymentToRemove is not null) ModePayments.Remove(modePaymentToRemove);
        }
        else
        {
            var editedModePayment = addEditModePaymentWindow.ModePayment;
            Log.Information(
                "Attempting to update mode payment id:\"{EditedModePaymentId}\", name:\"{EditedModePaymentName}\"",
                editedModePayment.Id, editedModePayment.Name);

            var (success, exception) = editedModePayment.AddOrEdit();
            if (success)
            {
                ModePayments!.AddAndSort(modePaymentToRemove, editedModePayment, s => s!.Name!);
                History.ModePaymentFk = editedModePayment.Id;

                Log.Information("Mode payment was successfully edited");
                var json = editedModePayment.ToJsonString();
                Log.Information("{Json}", json);

                MsgBox.Show(ModePaymentManagementResources.MessageBoxEditModePaymentSuccessMessage, MsgBoxImage.Check);
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MsgBox.Show(ModePaymentManagementResources.MessageBoxEditModePaymentErrorMessage, MsgBoxImage.Error);
            }
        }
    }

    private void ButtonPlace_OnClick(object sender, RoutedEventArgs e)
    {
        var place = History.PlaceFk?.ToISql<TPlace>();
        if (place?.CanBeDeleted is false)
        {
            MsgBox.Show(LocationManagementResources.MessageBoxPlaceCantEditMessage, MsgBoxImage.Error);
            return;
        }

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of AddEditPlaceWindow is created to handle adding or editing a place.
        // The place is set to null if the place is not found.
        var addEditLocationWindow = new AddEditLocationWindow();
        if (place is not null) addEditLocationWindow.SetPlace(place, false);

        var result = addEditLocationWindow.ShowDialog();
        if (result is not true) return;

        // ReSharper disable once HeapView.DelegateAllocation
        var oldPlace = PlacesCollection.FirstOrDefault(s => s.Id == History.PlaceFk);
        if (addEditLocationWindow.PlaceDeleted)
        {
            if (oldPlace is not null) PlacesCollection.Remove(oldPlace);

            return;
        }

        var editedPlace = addEditLocationWindow.Place;
        Log.Information("Attempting to update place id:\"{EditedPlaceId}\", name:\"{EditedPlaceName}\"", editedPlace.Id,
            editedPlace.Name);

        var (success, exception) = editedPlace.AddOrEdit();
        if (success)
        {
            PlacesCollection!.AddAndSort(oldPlace, editedPlace, s => s!.Name!);
            History.PlaceFk = editedPlace.Id;

            Log.Information("Place was successfully edited");

            // Loop crash
            // var json = editedPlace.ToJsonString();
            // Log.Information("{Json}", json);

            MsgBox.Show(LocationManagementResources.MessageBoxEditPlaceSuccessMessage, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(LocationManagementResources.MessageBoxEditPlaceErrorMessage, MsgBoxImage.Error);
        }
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The serviceProvider and items are set to null because they are not required in this context.
        // The ValidationResults list will store any validation errors detected during the process.
        var validationContext = new ValidationContext(History, serviceProvider: null, items: null);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Using 'var' keeps the code concise and readable, as the type (List<ValidationResult>)
        // is evident from the initialization. The result will still be compatible with any method
        // that expects an ICollection<ValidationResult>, as List<T> implements the ICollection interface.
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(History, validationContext, validationResults, true);

        if (!isValid)
        {
            var propertyError = validationResults.First();
            var propertyMemberName = propertyError.MemberNames.First();

            var messageErrorKey = propertyMemberName switch
            {
                nameof(THistory.AccountFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationAccountFkError),
                nameof(THistory.Description) => nameof(DetailedRecordManagementResources.MessageBoxValidationDescriptionError),
                nameof(THistory.CategoryTypeFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationCategoryTypeFkError),
                nameof(THistory.ModePaymentFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationModePaymentFkError),
                nameof(THistory.Value) => nameof(DetailedRecordManagementResources.MessageBoxValidationValueError),
                nameof(THistory.Date) => nameof(DetailedRecordManagementResources.MessageBoxValidationDateError),
                nameof(THistory.PlaceFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationPlaceFkError),
                _ => null
            };

            var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
                ? propertyError.ErrorMessage!
                : DetailedRecordManagementResources.ResourceManager.GetString(messageErrorKey)!;

            MsgBox.Show(localizedErrorMessage, MsgBoxImage.Error);
            return;
        }

        Log.Information("Attempting to inject the new history");

        if (History.IsPointed) History.DatePointed = DateTime.Now;
        else History.DatePointed = null;

        var (success, exception) = History.AddOrEdit();
        if (success)
        {
            Log.Information("History was successfully added");
            var json = History.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.Show(DetailedRecordManagementResources.MessageBoxAddHistorySuccessMessage, MsgBoxImage.Check);

            if (EditHistory)
            {
                if (History.BankTransferFk is not null)
                {
                    using var context = new DataBaseContext();
                    var bankTransfer = context.TBankTransfers.FirstOrDefault(s => s.Id == History.BankTransferFk);
                    if (bankTransfer is not null)
                    {
                        bankTransfer.MainReason = History.Description;
                        bankTransfer.Value = Math.Abs(History.Value ?? 0);
                        bankTransfer.AddOrEdit();
                    }
                }

                nameof(MainWindow.FrameBody).GoBack();
                return;
            }

            var response = MsgBox.Show(DetailedRecordManagementResources.MessageBoxAddHistoryQuestionMessage, MsgBoxImage.Question,
                MessageBoxButton.YesNoCancel);
            if (response is not MessageBoxResult.Yes) nameof(MainWindow.FrameBody).GoBack();

            History.Reset();
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(DetailedRecordManagementResources.MessageBoxAddHistoryErrorMessage, MsgBoxImage.Error);
        }
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).GoBack();

    private void ButtonDateNow_OnClick(object sender, RoutedEventArgs e)
        => History.Date = DateTime.Now;

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteHistoryQuestionMessage, MsgBoxImage.Question,
            MessageBoxButton.YesNoCancel);

        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the record \"{HistoryToDeleteDescriiption}\"", History.Description);
        var (success, exception) = History.Delete();

        if (success)
        {
            Log.Information("This Record was successfully removed");
            MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteHistoryNoUseSuccessMessage, MsgBoxImage.Check);

            DeleteBankTransfer();

            nameof(MainWindow.FrameBody).GoBack();
            return;
        }

        if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response = MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteUseRecordQuestionMessage,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information(
                "Attempting to remove this record \"{HistoryToDeleteDescriiption}\" with all relative element",
                History.Description);
            History.Delete(true);
            DeleteBankTransfer();
            Log.Information("This record and all relative element was successfully removed");

            MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteHistoryUseSuccessMessage, MsgBoxImage.Check);

            nameof(MainWindow.FrameBody).GoBack();
            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteHistoryErrorMessage, MsgBoxImage.Error);
    }

    private void Configuration_OnConfigurationChanged()
        => UpdateConfiguration();

    private void TextBoxValue_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text;
        var position = textBox.CaretIndex;

        _ = txt.ToDouble(out var value);
        History.Value = value;

        textBox.CaretIndex = position;
    }

    private void MapControl_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateTileLayer();

    private void SelectorCity_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox)return;

        // ReSharper disable once HeapView.ClosureAllocation
        if (comboBox.SelectedItem is not string city) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        var query = context.TPlaces.Where(s => s.IsOpen);

        IQueryable<TPlace> records;

        if (!string.IsNullOrEmpty(city))
        {
            records = city.Equals(EmptyStringTreeViewConverterResources.Unknown)
                ? query.Where(s => s.City == null)
                : query.Where(s => s.City == city);
        }
        else
        {
            records = query;
        }

        if (SelectedCountry is null)
        {
            // ReSharper disable HeapView.DelegateAllocation
            ComboBoxSelectorCountry.SelectionChanged -= SelectorCountry_OnSelectionChanged;
            SelectedCountry = records.First().Country;
            ComboBoxSelectorCountry.SelectionChanged += SelectorCountry_OnSelectionChanged;
            // ReSharper restore HeapView.DelegateAllocation
        }

        PlacesCollection.Clear();
        PlacesCollection.AddRangeAndSort(records, s => s.Name!);
    }

    private void SelectorCountry_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox)return;

        // ReSharper disable once HeapView.ClosureAllocation
        if (comboBox.SelectedItem is not string country) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        var query = context.TPlaces.Where(s => s.IsOpen);

        IQueryable<TPlace> records;

        if (!string.IsNullOrEmpty(country))
        {
            records = country.Equals(EmptyStringTreeViewConverterResources.Unknown)
                ? query.Where(s => s.Country == null)
                : query.Where(s => s.Country == country);
        }
        else
        {
            records = query;
        }

        var citiesResults = records.Select(s => EmptyStringTreeViewConverter.ToUnknown(s.City)).Distinct();

        // ReSharper disable HeapView.DelegateAllocation
        ComboBoxSelectorCity.SelectionChanged -= SelectorCity_OnSelectionChanged;
        ComboBoxSelectorPlace.SelectionChanged -= SelectorPlace_OnSelectionChanged;
        // ReSharper restore HeapView.DelegateAllocation

        CitiesCollection.Clear();
        CitiesCollection.AddRangeAndSort(citiesResults, s => s);

        PlacesCollection.Clear();
        PlacesCollection.AddRangeAndSort(records, s => s.Name!);

        // ReSharper disable HeapView.DelegateAllocation
        ComboBoxSelectorCity.SelectionChanged += SelectorCity_OnSelectionChanged;
        ComboBoxSelectorPlace.SelectionChanged += SelectorPlace_OnSelectionChanged;
        // ReSharper restore HeapView.DelegateAllocation
    }

    private void SelectorPlace_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var place = History.PlaceFk?.ToISql<TPlace>();
        UpdateMapPoint(place);

        // ReSharper disable HeapView.DelegateAllocation
        ComboBoxSelectorCountry.SelectionChanged -= SelectorCountry_OnSelectionChanged;
        ComboBoxSelectorCity.SelectionChanged -= SelectorCity_OnSelectionChanged;
        // ReSharper restore HeapView.DelegateAllocation

        var country = string.IsNullOrEmpty(place?.Country)
            ? EmptyStringTreeViewConverterResources.Unknown
            : place.Country;

        var city = string.IsNullOrEmpty(place?.City)
            ? EmptyStringTreeViewConverterResources.Unknown
            : place.City;

        ComboBoxSelectorCountry.SelectedItem = country;
        ComboBoxSelectorCity.SelectedItem = city;
        History.PlaceFk = place?.Id;

        // ReSharper disable HeapView.DelegateAllocation
        ComboBoxSelectorCountry.SelectionChanged += SelectorCountry_OnSelectionChanged;
        ComboBoxSelectorCity.SelectionChanged += SelectorCity_OnSelectionChanged;
        // ReSharper restore HeapView.DelegateAllocation
    }

    private void SelectorTile_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateTileLayer();

    private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text.Insert(textBox.SelectionStart, e.Text);

        if (txt.Equals("-") || txt.Equals("+"))
        {
            e.Handled = false;
            return;
        }

        var canConvert = txt.ToDouble(out _);

        e.Handled = !canConvert;
    }

    private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var textBox = (TextBox)sender;
        var textBeforeEdit = textBox.Text;
        var caretPosition = textBox.CaretIndex;

        var characterToDelete = e.Key switch
        {
            Key.Delete when caretPosition < textBeforeEdit.Length => textBox.Text.Substring(caretPosition, 1),
            Key.Back when caretPosition > 0 => textBox.Text.Substring(caretPosition - 1, 1),
            _ => ""
        };

        if (characterToDelete != "." && characterToDelete != ",") return;

        var textAfterEdit = textBeforeEdit.Remove(caretPosition - (e.Key == Key.Back ? 1 : 0), 1); // Simulate deletion

        textAfterEdit = textAfterEdit.Replace(',', '.');
        if (double.TryParse(textAfterEdit, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
        {
            return;
        }

        e.Handled = true;
        textBox.CaretIndex = caretPosition;
    }

    #endregion

    #region Function

    private void DeleteBankTransfer()
    {
        if (History.BankTransferFk is null) return;

        var bankTransfer = History.BankTransferFk?.ToISql<TBankTransfer>();
        bankTransfer?.Delete(true);
    }

    public void SetTHistory(THistory history)
    {
        var place = history.PlaceFk?.ToISql<TPlace>();
        SelectedCountry = EmptyStringTreeViewConverter.ToUnknown(place?.Country);
        SelectedCity = EmptyStringTreeViewConverter.ToUnknown(place?.City);

        history.CopyPropertiesTo(History);
        EditHistory = true;
    }

    private void UpdateConfiguration()
    {
        var configuration = Config.Configuration;

        UpdateLanguage(configuration.Interface.Language!);

        TimePicker.Is24Hours = configuration.Interface.Clock.Is24Hours;

        var backColor = Utils.Resources.GetMaterialDesignPaperMapsUiStylesColor();
        MapControl.Map.BackColor = backColor;
    }

    private void UpdateLanguage(string cultureInfoCode)
    {
        var xmlLanguage = XmlLanguage.GetLanguage(cultureInfoCode);
        DatePicker.Language = xmlLanguage;

        ComboBoxAccountHintAssist = DetailedRecordManagementResources.ComboBoxAccountHintAssist;
        TextBoxDescriptionHintAssist = DetailedRecordManagementResources.TextBoxDescriptionHintAssist;
        ComboBoxCategoryTypeHintAssist = DetailedRecordManagementResources.ComboBoxCategoryTypeHintAssist;
        ComboBoxModePaymentHintAssist = DetailedRecordManagementResources.ComboBoxModePaymentHintAssist;
        TextBoxValueHintAssist = DetailedRecordManagementResources.TextBoxValueHintAssist;
        DatePickerWhenHintAssist = DetailedRecordManagementResources.DatePickerWhenHintAssist;
        TimePickerWhenHintAssist = DetailedRecordManagementResources.TimePickerWhenHintAssist;
        ComboBoxPlaceCountryHintAssist = DetailedRecordManagementResources.ComboBoxPlaceCountryHintAssist;
        ComboBoxPlaceCityHintAssist = DetailedRecordManagementResources.ComboBoxPlaceCityHintAssist;
        ComboBoxPlaceHintAssist = DetailedRecordManagementResources.ComboBoxPlaceHintAssist;
        CheckBoxPointedContent = DetailedRecordManagementResources.PointedOperation;
        ComboBoxBackgroundHintAssist = DetailedRecordManagementResources.ComboBoxBackgroundHintAssist;
        ButtonValidContent = DetailedRecordManagementResources.ButtonValidText;
        ButtonDeleteContent = DetailedRecordManagementResources.ButtonDeleteText;
        ButtonCancelContent = DetailedRecordManagementResources.ButtonCancelText;
    }

    private void UpdateMapPoint(TPlace? place)
    {
        PlaceLayer.Clear();

        if (place is null)
        {
            MapControl.Refresh();
            return;
        }

        var symbolStyle = place.IsOpen
            ? MapsuiStyleExtensions.RedMarkerStyle
            : MapsuiStyleExtensions.BlueMarkerStyle;

        var pointFeature = place.ToFeature(symbolStyle);

        PlaceLayer.Add(pointFeature);
        MapControl.Map.Navigator.CenterOnAndZoomTo(pointFeature.Point);
    }

    private void UpdateTileLayer()
    {
        const string layerName = "Background";

        var httpTileSource = BruTile.Predefined.KnownTileSources.Create(KnownTileSourceSelected);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Allocation is necessary because a TileLayer is immutable after it is created.
        // This means that once the TileSource of a TileLayer is set,
        // it can no longer be changed directly.
        // In this case, to change the source, a new TileLayer must be created
        // and added to the menu.
        var tileLayer = new TileLayer(httpTileSource);
        tileLayer.Name = layerName;

        var layers = MapControl?.Map.Layers.FindLayer(layerName);
        if (layers is not null) MapControl?.Map.Layers.Remove(layers.ToArray());

        MapControl?.Map.Layers.Insert(0, tileLayer);
    }

    #endregion
}