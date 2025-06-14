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
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Objects;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.AddEditAccount;
using MyExpenses.SharedUtils.Resources.Resx.CategoryTypesManagement;
using MyExpenses.SharedUtils.Resources.Resx.LocationManagement;
using MyExpenses.SharedUtils.Resources.Resx.ModePaymentManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.DateTimes;
using MyExpenses.Utils.Maps;
using MyExpenses.Utils.Resources.Resx.Converters.EmptyStringTreeViewConverter;
using MyExpenses.Utils.Sql;
using MyExpenses.Utils.Strings;
using MyExpenses.Wpf.Converters;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditRecurrentExpenseWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;
using MyExpenses.Wpf.Windows.LocationManagementWindows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditRecurrentExpenseWindow
{
    #region DependencyProperty

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.Register(nameof(WindowTitle),
        typeof(string), typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string WindowTitle
    {
        get => (string)GetValue(WindowTitleProperty);
        set => SetValue(WindowTitleProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ComboBoxAccountHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxAccountHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string ComboBoxAccountHintAssist
    {
        get => (string)GetValue(ComboBoxAccountHintAssistProperty);
        set => SetValue(ComboBoxAccountHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ComboBoxBackgroundHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxBackgroundHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string ComboBoxBackgroundHintAssist
    {
        get => (string)GetValue(ComboBoxBackgroundHintAssistProperty);
        set => SetValue(ComboBoxBackgroundHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxDescriptionHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxDescriptionHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string TextBoxDescriptionHintAssist
    {
        get => (string)GetValue(TextBoxDescriptionHintAssistProperty);
        set => SetValue(TextBoxDescriptionHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxNoteHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxNoteHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string TextBoxNoteHintAssist
    {
        get => (string)GetValue(TextBoxNoteHintAssistProperty);
        set => SetValue(TextBoxNoteHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ComboBoxModePaymentHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxModePaymentHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string ComboBoxModePaymentHintAssist
    {
        get => (string)GetValue(ComboBoxModePaymentHintAssistProperty);
        set => SetValue(ComboBoxModePaymentHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ComboBoxCategoryTypeHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxCategoryTypeHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string ComboBoxCategoryTypeHintAssist
    {
        get => (string)GetValue(ComboBoxCategoryTypeHintAssistProperty);
        set => SetValue(ComboBoxCategoryTypeHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxValueHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxValueHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string TextBoxValueHintAssist
    {
        get => (string)GetValue(TextBoxValueHintAssistProperty);
        set => SetValue(TextBoxValueHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ComboBoxFrequencyHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxFrequencyHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string ComboBoxFrequencyHintAssist
    {
        get => (string)GetValue(ComboBoxFrequencyHintAssistProperty);
        set => SetValue(ComboBoxFrequencyHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxRecursiveTotalHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxRecursiveTotalHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string TextBoxRecursiveTotalHintAssist
    {
        get => (string)GetValue(TextBoxRecursiveTotalHintAssistProperty);
        set => SetValue(TextBoxRecursiveTotalHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxRecursiveCountHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxRecursiveCountHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string TextBoxRecursiveCountHintAssist
    {
        get => (string)GetValue(TextBoxRecursiveCountHintAssistProperty);
        set => SetValue(TextBoxRecursiveCountHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty DatePickerStartDateHintAssistProperty =
        DependencyProperty.Register(nameof(DatePickerStartDateHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string DatePickerStartDateHintAssist
    {
        get => (string)GetValue(DatePickerStartDateHintAssistProperty);
        set => SetValue(DatePickerStartDateHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxNextDueDateHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxNextDueDateHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string TextBoxNextDueDateHintAssist
    {
        get => (string)GetValue(TextBoxNextDueDateHintAssistProperty);
        set => SetValue(TextBoxNextDueDateHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty CheckBoxForceDeactivateProperty =
        DependencyProperty.Register(nameof(CheckBoxForceDeactivate), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string CheckBoxForceDeactivate
    {
        get => (string)GetValue(CheckBoxForceDeactivateProperty);
        set => SetValue(CheckBoxForceDeactivateProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty CheckBoxIsActiveProperty =
        DependencyProperty.Register(nameof(CheckBoxIsActive), typeof(string), typeof(AddEditRecurrentExpenseWindow),
            new PropertyMetadata(default(string)));

    public string CheckBoxIsActive
    {
        get => (string)GetValue(CheckBoxIsActiveProperty);
        set => SetValue(CheckBoxIsActiveProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ComboBoxPlaceCountryHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxPlaceCountryHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string ComboBoxPlaceCountryHintAssist
    {
        get => (string)GetValue(ComboBoxPlaceCountryHintAssistProperty);
        set => SetValue(ComboBoxPlaceCountryHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ComboBoxPlaceCityHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxPlaceCityHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string ComboBoxPlaceCityHintAssist
    {
        get => (string)GetValue(ComboBoxPlaceCityHintAssistProperty);
        set => SetValue(ComboBoxPlaceCityHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ComboBoxPlaceHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxPlaceHintAssist), typeof(string),
            typeof(AddEditRecurrentExpenseWindow), new PropertyMetadata(default(string)));

    public string ComboBoxPlaceHintAssist
    {
        get => (string)GetValue(ComboBoxPlaceHintAssistProperty);
        set => SetValue(ComboBoxPlaceHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonValidContentProperty =
        DependencyProperty.Register(nameof(ButtonValidContent), typeof(string), typeof(AddEditRecurrentExpenseWindow),
            new PropertyMetadata(default(string)));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonDeleteContentProperty =
        DependencyProperty.Register(nameof(ButtonDeleteContent), typeof(string), typeof(AddEditRecurrentExpenseWindow),
            new PropertyMetadata(default(string)));

    public string ButtonDeleteContent
    {
        get => (string)GetValue(ButtonDeleteContentProperty);
        set => SetValue(ButtonDeleteContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(AddEditRecurrentExpenseWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

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

    public ObservableCollection<TAccount> Accounts { get; }
    public ObservableCollection<TCategoryType> CategoryTypes { get; }
    public ObservableCollection<TModePayment> ModePayments { get; }
    public ObservableCollection<TRecursiveFrequency> RecursiveFrequencies { get; }

    public ObservableCollection<string> CountriesCollection { get; }

    public ObservableCollection<string> CitiesCollection { get; }
    public ObservableCollection<TPlace> PlacesCollection { get; }

    public string SelectedValuePathAccount { get; } = nameof(TAccount.Id);
    public string DisplayMemberPathAccount { get; } = nameof(TAccount.Name);
    public string SelectedValuePathCategoryType { get; } = nameof(TCategoryType.Id);
    public string DisplayMemberPathCategoryType { get; } = nameof(TCategoryType.Name);
    public string SelectedValuePathModePayment { get; } = nameof(TModePayment.Id);
    public string DisplayMemberPathModePayment { get; } = nameof(TModePayment.Name);
    public string SelectedValuePathFrequencyFk { get; } = nameof(TRecursiveFrequency.Id);
    public string DisplayMemberPathFrequencyFk { get; } = nameof(TRecursiveFrequency.Frequency);
    public string SelectedValuePathPlace { get; } = nameof(TPlace.Id);
    public string DisplayMemberPathPlaceName { get; } = nameof(TPlace.Name);

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    private WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(TPlace) };
    public List<KnownTileSource> KnownTileSources { get; }
    public KnownTileSource KnownTileSourceSelected { get; set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TRecursiveExpense RecursiveExpense { get; } = new();

    #endregion

    public AddEditRecurrentExpenseWindow()
    {
        KnownTileSources = [..MapsuiMapExtensions.GetAllKnowTileSource()];

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];
        CategoryTypes = [..context.TCategoryTypes.OrderBy(s => s.Name)];
        ModePayments = [..context.TModePayments.OrderBy(s => s.Name)];
        RecursiveFrequencies = [..context.TRecursiveFrequencies.OrderBy(s => s.Id)];

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
        UpdaterLanguage();

        MapControl.Map = map;

        this.SetWindowCornerPreference();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonAccount_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var addEditAccountWindow = new AddEditAccountWindow();

        var account = RecursiveExpense.AccountFk?.ToISql<TAccount>();
        if (account is not null) addEditAccountWindow.SetTAccount(account);

        addEditAccountWindow.ShowDialog();
        if (addEditAccountWindow.DialogResult is not true) return;

        if (addEditAccountWindow.DeleteAccount)
        {
            // ReSharper disable once HeapView.DelegateAllocation
            var accountToRemove = Accounts.FirstOrDefault(s => s.Id == RecursiveExpense.AccountFk);
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

                MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxEditAccountSuccessMessage, MsgBoxImage.Check);

                // ReSharper disable once HeapView.DelegateAllocation
                var accountToRemove = Accounts.FirstOrDefault(s => s.Id == RecursiveExpense.AccountFk);
                Accounts!.AddAndSort(accountToRemove, editedAccount, s => s?.Name!);

                RecursiveExpense.AccountFk = editedAccount.Id;
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxEditAccountErrorMessage, MsgBoxImage.Warning);
            }
        }
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonCategoryType_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var addEditCategoryTypeWindow = new AddEditCategoryTypeWindow();
        var categoryType = RecursiveExpense.CategoryTypeFk?.ToISql<TCategoryType>();
        if (categoryType is not null) addEditCategoryTypeWindow.SetTCategoryType(categoryType);

        var result = addEditCategoryTypeWindow.ShowDialog();
        if (result is not true) return;

        if (addEditCategoryTypeWindow.CategoryTypeDeleted)
        {
            // ReSharper disable once HeapView.DelegateAllocation
            var categoryTypeToRemove = CategoryTypes.FirstOrDefault(s => s.Id == RecursiveExpense.CategoryTypeFk);
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
                using var context = new DataBaseContext();
                editedCategoryTypeDeepCopy.ColorFkNavigation =
                    context.TColors.FirstOrDefault(s => s.Id == editedCategoryTypeDeepCopy.ColorFk);

                CategoryTypes!.AddAndSort(categoryType, editedCategoryTypeDeepCopy, s => s!.Name!);

                Log.Information("Category type was successfully edited");
                var json = editedCategoryTypeDeepCopy.ToJsonString();
                Log.Information("{Json}", json);

                MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxCategoryTypeEditSuccessTitle,
                    CategoryTypesManagementResources.MessageBoxCategoryTypeEditSuccessMessage, MsgBoxImage.Check);
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxCategoryTypeEditErrorTitle,
                    CategoryTypesManagementResources.MessageBoxCategoryTypeEditErrorMessage, MsgBoxImage.Error);
            }
        }
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the recursive expense \"{RecursiveExpenseDescription}\"", RecursiveExpense.Description);
        var (success, exception) = RecursiveExpense.Delete();

        if (success)
        {
            Log.Information("Recursive expense was successfully removed");
            MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxDeleteRecursiveExpenseNoUseSuccess,
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

            response = MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxDeleteRecursiveExpenseUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information("Attempting to remove the recursive expense \"{RecursiveExpenseDescription}\" with all relative element",
                RecursiveExpense.Description);
            RecursiveExpense.Delete(true);
            Log.Information("Recursive expense and all relative element was successfully removed");
            MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxDeleteRecursiveExpenseUseSuccess,
                MsgBoxImage.Check);

            // RecursiveExpenseDeleted = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxDeleteRecursiveExpenseError, MsgBoxImage.Error);
    }

    private void ButtonModePayment_OnClick(object sender, RoutedEventArgs e)
    {
        var modePayment = RecursiveExpense.ModePaymentFk?.ToISql<TModePayment>();
        if (modePayment?.CanBeDeleted is false)
        {
            MsgBox.MsgBox.Show(ModePaymentManagementResources.MessageBoxModePaymentCantEditTitle,
                ModePaymentManagementResources.MessageBoxModePaymentCantEditMessage, MsgBoxImage.Error);
            return;
        }

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var addEditModePaymentWindow = new AddEditModePaymentWindow();
        if (modePayment is not null) addEditModePaymentWindow.SetTModePayment(modePayment);

        var result = addEditModePaymentWindow.ShowDialog();
        if (result is not true) return;

        // ReSharper disable once HeapView.DelegateAllocation
        var modePaymentToRemove = ModePayments.FirstOrDefault(s => s.Id == RecursiveExpense.ModePaymentFk);
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
                RecursiveExpense.ModePaymentFk = editedModePayment.Id;

                Log.Information("Mode payment was successfully edited");
                var json = editedModePayment.ToJsonString();
                Log.Information("{Json}", json);

                MsgBox.MsgBox.Show(ModePaymentManagementResources.MessageBoxEditModePaymentSuccessMessage, MsgBoxImage.Check);
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MsgBox.MsgBox.Show(ModePaymentManagementResources.MessageBoxEditModePaymentErrorMessage, MsgBoxImage.Error);
            }
        }
    }

    private void ButtonPlace_OnClick(object sender, RoutedEventArgs e)
    {
        var place = RecursiveExpense.PlaceFk?.ToISql<TPlace>();
        if (place?.CanBeDeleted is false)
        {
            MsgBox.MsgBox.Show(LocationManagementResources.MessageBoxPlaceCantEditMessage, MsgBoxImage.Error);
            return;
        }

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var addEditLocationWindow = new AddEditLocationWindow();
        if (place is not null) addEditLocationWindow.SetPlace(place, false);

        var result = addEditLocationWindow.ShowDialog();
        if (result is not true) return;

        // ReSharper disable once HeapView.DelegateAllocation
        var oldPlace = PlacesCollection.FirstOrDefault(s => s.Id == RecursiveExpense.PlaceFk);
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
            RecursiveExpense.PlaceFk = editedPlace.Id;

            Log.Information("Place was successfully edited");

            // Loop crash
            // var json = editedPlace.ToJsonString();
            // Log.Information("{Json}", json);

            MsgBox.MsgBox.Show(LocationManagementResources.MessageBoxEditPlaceSuccessMessage, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.MsgBox.Show(LocationManagementResources.MessageBoxEditPlaceErrorMessage, MsgBoxImage.Error);
        }
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

            MsgBox.MsgBox.Show(localizedErrorMessage, MsgBoxImage.Error);
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

            MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxAddRecursiveExpenseSuccess, MsgBoxImage.Check);

            if (EditRecurrentExpense)
            {
                DialogResult = true;
                Close();
                return;
            }

            var response = MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxAddRecursiveExpenseQuestion, MsgBoxImage.Question,
                MessageBoxButton.YesNoCancel);
            if (response is not MessageBoxResult.Yes) Close();

            RecursiveExpense.Reset();
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.MsgBox.Show(AddEditRecurrentExpenseWindowResources.MessageBoxAddRecursiveExpenseError, MsgBoxImage.Error);
        }
    }

    private void DatePicker_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        => UpdateNextDueDate();

    private void Interface_OnLanguageChanged()
        => UpdaterLanguage();

    private void MapControl_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateTileLayer();

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateNextDueDate();

    private void SelectorCity_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox) return;

        // ReSharper disable once HeapView.ClosureAllocation
        if (comboBox.SelectedItem is not string city) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
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
        if (sender is not ComboBox comboBox) return;

        // ReSharper disable once HeapView.ClosureAllocation
        if (comboBox.SelectedItem is not string country) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
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
        var place = RecursiveExpense.PlaceFk?.ToISql<TPlace>();
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
        RecursiveExpense.PlaceFk = place?.Id;

        // ReSharper disable HeapView.DelegateAllocation
        ComboBoxSelectorCountry.SelectionChanged += SelectorCountry_OnSelectionChanged;
        ComboBoxSelectorCity.SelectionChanged += SelectorCity_OnSelectionChanged;
        // ReSharper restore HeapView.DelegateAllocation
    }

    private void SelectorTile_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateTileLayer();

    private void TextBoxRecursiveTotal_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            RecursiveExpense.RecursiveTotal = null;
        }

        UpdateIsActive();
    }

    private void TextBoxRecursiveCount_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            RecursiveExpense.RecursiveCount = 0;
        }

        UpdateIsActive();
    }

    private void TextBoxValueDoubleOnly_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text;
        var position = textBox.CaretIndex;

        _ = txt.ToDouble(out var value);
        RecursiveExpense.Value = value;

        textBox.CaretIndex = position;
    }

    private void UIElementDoubleOnly_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
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

    private void UIElementDoubleOnly_OnPreviewKeyDown(object sender, KeyEventArgs e)
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

        if (characterToDelete != "." && characterToDelete != ",")
        {
            return;
        }

        var textAfterEdit = textBeforeEdit.Remove(caretPosition - (e.Key == Key.Back ? 1 : 0), 1); // Simulate deletion

        textAfterEdit = textAfterEdit.Replace(',', '.');
        if (double.TryParse(textAfterEdit, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
        {
            return;
        }

        e.Handled = true;
        textBox.CaretIndex = caretPosition;
    }

    private void UIElementIntOnlyRecursiveTotal_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text.Insert(textBox.SelectionStart, e.Text);

        var success = txt.ToInt(out var value);

        if (success) success = value! > 0;

        e.Handled = !success;
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

    private void UpdateIsActive()
    {
        if (RecursiveExpense.RecursiveTotal is null) RecursiveExpense.IsActive = true;

        RecursiveExpense.IsActive = RecursiveExpense.RecursiveTotal > RecursiveExpense.RecursiveCount;
    }

    private void UpdaterLanguage()
    {
        var cultureInfoCode = CultureInfo.CurrentCulture.Name;
        var xmlLanguage = XmlLanguage.GetLanguage(cultureInfoCode);
        DatePicker.Language = xmlLanguage;

        WindowTitle = AddEditRecurrentExpenseWindowResources.WindowTitle;

        ComboBoxAccountHintAssist = AddEditRecurrentExpenseWindowResources.ComboBoxAccountHintAssist;
        ComboBoxBackgroundHintAssist = AddEditRecurrentExpenseWindowResources.ComboBoxBackgroundHintAssist;
        TextBoxDescriptionHintAssist = AddEditRecurrentExpenseWindowResources.TextBoxDescriptionHintAssist;
        TextBoxNoteHintAssist = AddEditRecurrentExpenseWindowResources.TextBoxNoteHintAssist;
        ComboBoxModePaymentHintAssist = AddEditRecurrentExpenseWindowResources.ComboBoxModePaymentHintAssist;
        ComboBoxCategoryTypeHintAssist = AddEditRecurrentExpenseWindowResources.ComboBoxCategoryTypeHintAssist;
        TextBoxValueHintAssist = AddEditRecurrentExpenseWindowResources.TextBoxValueHintAssist;
        ComboBoxFrequencyHintAssist = AddEditRecurrentExpenseWindowResources.ComboBoxFrequencyHintAssist;
        TextBoxRecursiveTotalHintAssist = AddEditRecurrentExpenseWindowResources.TextBoxRecursiveTotalHintAssist;
        TextBoxRecursiveCountHintAssist = AddEditRecurrentExpenseWindowResources.TextBoxRecursiveCountHintAssist;
        DatePickerStartDateHintAssist = AddEditRecurrentExpenseWindowResources.DatePickerStartDateHintAssist;
        TextBoxNextDueDateHintAssist = AddEditRecurrentExpenseWindowResources.TextBoxNextDueDateHintAssist;
        CheckBoxForceDeactivate = AddEditRecurrentExpenseWindowResources.CheckBoxForceDeactivate;
        CheckBoxIsActive = AddEditRecurrentExpenseWindowResources.CheckBoxIsActive;
        ComboBoxPlaceCountryHintAssist = AddEditRecurrentExpenseWindowResources.ComboBoxPlaceCountryHintAssist;
        ComboBoxPlaceCityHintAssist = AddEditRecurrentExpenseWindowResources.ComboBoxPlaceCityHintAssist;
        ComboBoxPlaceHintAssist = AddEditRecurrentExpenseWindowResources.ComboBoxPlaceHintAssist;

        ButtonValidContent = AddEditRecurrentExpenseWindowResources.ButtonValidContent;
        ButtonDeleteContent = AddEditRecurrentExpenseWindowResources.ButtonDeleteContent;
        ButtonCancelContent = AddEditRecurrentExpenseWindowResources.ButtonCancelContent;
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

    private void UpdateNextDueDate()
    {
        // ReSharper disable once HeapView.DelegateAllocation
        var selectedFrequency = RecursiveFrequencies.FirstOrDefault(s => s.Id == RecursiveExpense.FrequencyFk);
        if (selectedFrequency is null && !EditRecurrentExpense)
        {
            RecursiveExpense.NextDueDate = RecursiveExpense.StartDate;
            return;
        }

        DateOnly dateOnly;
        if (EditRecurrentExpense)
        {
            var cycle = RecursiveExpense.RecursiveCount < 1 ? 1 : RecursiveExpense.RecursiveCount;
            dateOnly = RecursiveExpense.ERecursiveFrequency
                .CalculateNextDueDate(RecursiveExpense.StartDate, TModePayment.GetModePayment(RecursiveExpense.ModePaymentFk), cycle);
        }
        else
        {
            var now = DateOnly.FromDateTime(DateTime.Now);

            dateOnly = RecursiveExpense.StartDate >= now
                ? RecursiveExpense.StartDate.AdjustForWeekends()
                : RecursiveExpense.ERecursiveFrequency.CalculateNextDueDate(RecursiveExpense.StartDate, TModePayment.GetModePayment(RecursiveExpense.ModePaymentFk));
        }

        RecursiveExpense.NextDueDate = dateOnly;
    }

    private void UpdateTileLayer()
    {
        const string layerName = "Background";

        var httpTileSource = BruTile.Predefined.KnownTileSources.Create(KnownTileSourceSelected);
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var tileLayer = new TileLayer(httpTileSource);
        tileLayer.Name = layerName;

        var layers = MapControl?.Map.Layers.FindLayer(layerName);
        if (layers is not null) MapControl?.Map.Layers.Remove(layers.ToArray());

        MapControl?.Map.Layers.Insert(0, tileLayer);
    }

    #endregion
}