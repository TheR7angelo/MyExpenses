using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AddEditCategoryTypesContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AddEditCategoryTypesContentPage
{
    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(AddEditCategoryTypesContentPage));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(AddEditCategoryTypesContentPage));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty SelectedHexadecimalColorCodeProperty =
        BindableProperty.Create(nameof(SelectedHexadecimalColorCode), typeof(string),
            typeof(AddEditCategoryTypesContentPage), "#00000000");

    public string SelectedHexadecimalColorCode
    {
        get => (string)GetValue(SelectedHexadecimalColorCodeProperty);
        set => SetValue(SelectedHexadecimalColorCodeProperty, value);
    }

    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(nameof(SelectedColor),
        typeof(TColor), typeof(AddEditCategoryTypesContentPage));

    public TColor? SelectedColor
    {
        get => (TColor)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public static readonly BindableProperty LabelTextColorProperty = BindableProperty.Create(nameof(LabelTextColor),
        typeof(string), typeof(AddEditCategoryTypesContentPage));

    public string LabelTextColor
    {
        get => (string)GetValue(LabelTextColorProperty);
        set => SetValue(LabelTextColorProperty, value);
    }

    public static readonly BindableProperty CategoryTypeNameProperty = BindableProperty.Create(nameof(CategoryTypeName),
        typeof(string), typeof(AddEditCategoryTypesContentPage));

    public string CategoryTypeName
    {
        get => (string)GetValue(CategoryTypeNameProperty);
        set => SetValue(CategoryTypeNameProperty, value);
    }

    public int MaxLength { get; }
    public ObservableCollection<TColor> Colors { get; } = [];
    public ObservableCollection<VCategory> Categories { get; } = [];

    public ICommand BackCommand { get; init; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // TaskCompletionSource is intentionally allocated here as it is the fundamental mechanism
    // for creating and controlling the completion of the Task exposed by `ResultDialog`.
    // This object is required to manually signal task completion (`SetResult`, `SetException`, etc.)
    // when the operation is resolved, ensuring proper asynchronous flow.
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public AddEditCategoryTypesContentPage()
    {
        MaxLength = Utils.Converters.MaxLengthConverter.Convert(typeof(TCategoryType), nameof(TCategoryType.Name));

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // ReSharper disable once HeapView.DelegateAllocation
        // The Command object is explicitly created here to handle the user's interaction with the UI.
        // This allocation is necessary because `Command` encapsulates the behavior (in this case, `OnBackCommandPressed`)
        // and binds it to the associated UI element, such as a Button or a gesture.
        // This ensures proper separation between the UI and logic layers.
        BackCommand = new Command(OnBackCommandPressed);

        RefreshCollection();

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonValid_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonValid();

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void OnBackCommandPressed()
        => _ = HandleBackCommand();

    private void PickerColor_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        SelectedHexadecimalColorCode = SelectedColor is null
            ? "#00000000"
            : SelectedColor.HexadecimalColorCode!;
    }

    private void TapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => _ = HandleTapGestureRecognizer(sender);

    #endregion

    #region Function

    private async Task HandleBackCommand()
    {
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private async Task HandleButtonValid()
    {
        var validate = await ValidateCategoryType();
        if (!validate) return;

        var response = await DisplayAlert(
            AddEditCategoryTypesContentPageResources.MesageBoxAddNewCategoryTypeQuestionTitle,
            string.Format(AddEditCategoryTypesContentPageResources.MesageBoxAddNewCategoryTypeQuestionMessage,
                CategoryTypeName),
            AddEditCategoryTypesContentPageResources.MesageBoxAddNewCategoryTypeQuestionYesButton,
            AddEditCategoryTypesContentPageResources.MesageBoxAddNewCategoryTypeQuestionNoButton);
        if (!response) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Using an object initializer here ensures that the TCategoryType instance is constructed
        // and all required properties (Name, ColorFk, and DateAdded) are initialized in one concise
        // and readable statement. This approach minimizes the chance of missing a property initialization.
        // ReSharper disable once HeapView.ClosureAllocation
        var newCategoryTypeType = new TCategoryType
        {
            Name = CategoryTypeName,
            ColorFk = SelectedColor?.Id,
            DateAdded = DateTime.Now
        };

        var json = newCategoryTypeType.ToJson();
        Log.Information("Attempt to add new category type : {CategoryType}", json);
        var (success, exception) = newCategoryTypeType.AddOrEdit();
        if (success)
        {
            Log.Information("New category type was successfully added");

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
            // This context provides the connection to the database and allows querying or updating data.
            // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
            await using var context = new DataBaseContext();
            var newVCategory = context.VCategories.First(s => s.Id.Equals(newCategoryTypeType.Id));
            Categories.AddAndSort(newVCategory, s => s.CategoryName!);

            CategoryTypeName = string.Empty;
            SelectedColor = null;

            await DisplayAlert(
                AddEditCategoryTypesContentPageResources.MesageBoxAddNewCategoryTypeSuccessTitle,
                AddEditCategoryTypesContentPageResources.MesageBoxAddNewCategoryTypeSuccessMessage,
                AddEditCategoryTypesContentPageResources.MesageBoxAddNewCategoryTypeSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while adding new category type");
            await DisplayAlert(
                AddEditCategoryTypesContentPageResources.MesageBoxAddNewCategoryTypeErrorTitle,
                AddEditCategoryTypesContentPageResources.MesageBoxAddNewCategoryTypeErrorMessage,
                AddEditCategoryTypesContentPageResources.MesageBoxAddNewCategoryTypeErrorOkButton);
        }
    }

    private async Task HandleCategoryTypeDelete(TCategoryType categoryType)
    {
        var (success, exception) = categoryType.Delete(true);
        DashBoardContentPage.Instance.RefreshAccountTotal();

        CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
        if (success)
        {
            Log.Information("Category type and all related records were successfully deleted");
            await DisplayAlert(
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeDeleteSuccessTitle,
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeDeleteSuccessMessage,
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeDeleteSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while deleting category type");
            await DisplayAlert(
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeDeleteErrorTitle,
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeDeleteErrorMessage,
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeDeleteErrorOkButton);
        }
    }

    private async Task HandleCategoryTypeEdit(TCategoryType categoryType)
    {
        var (success, exception) = categoryType.AddOrEdit();
        if (success)
        {
            Log.Information("Category type was successfully edited");
            await DisplayAlert(
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditSuccessTitle,
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditSuccessMessage,
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while editing Category type");
            await DisplayAlert(
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditErrorTitle,
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditErrorMessage,
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditErrorOkButton);
        }
    }

    private async Task HandleTapGestureRecognizer(object? sender)
    {
        if (sender is not Border border) return;
        if (border.BindingContext is not VCategory category) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // A new instance of CustomPopupEditCategory is explicitly created here to allow customization
        // of the popup's behavior with specific properties such as MaxLength and CanDelete. This allocation
        // is necessary as each invocation may require a uniquely configured popup based on the application's
        // state or user interactions. The usage of an object initializer ensures these properties are set
        // immediately, avoiding uninitialized or invalid states for the popup.
        var customPopupEditCategory = new CustomPopupEditCategory { MaxLenght = MaxLength, CanDelete = true };
        customPopupEditCategory.SetVCategory(category);
        await this.ShowPopupAsync(customPopupEditCategory);

        var result = await customPopupEditCategory.ResultDialog;
        if (result is ECustomPopupEntryResult.Cancel) return;

        category.CategoryName = customPopupEditCategory.EntryText;
        category.ColorFk = customPopupEditCategory.SelectedColor?.Id;

        await HandleVCategoryResult(category, result);
        RefreshCategories();
    }

    private async Task HandleVCategoryResult(VCategory vCategory, ECustomPopupEntryResult result)
    {
        var json = vCategory.ToJson();
        var tCategory = vCategory.Id.ToISql<TCategoryType>()!;
        tCategory.Name = vCategory.CategoryName;
        tCategory.ColorFk = vCategory.ColorFk;

        if (result is ECustomPopupEntryResult.Valid)
        {
            var validate = await ValidateCategoryType(vCategory.CategoryName);
            if (!validate) return;

            var response = await DisplayAlert(
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditQuestionTitle,
                string.Format(AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditQuestionMessage, Environment.NewLine),
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditQuestionYesButton,
                AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditQuestionNoButton);
            if (!response) return;

            Log.Information("Attempt to edit category type : {Category}", json);
            await HandleCategoryTypeEdit(tCategory);

            return;
        }

        var deleteResponse = await DisplayAlert(
            AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditDeleteQuestionTitle,
            string.Format(AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditDeleteQuestionMessage, Environment.NewLine),
            AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditDeleteQuestionYesButton,
            AddEditCategoryTypesContentPageResources.MessageBoxHandleCategoryTypeEditDeleteQuestionNoButton);

        if (!deleteResponse) return;

        await Task.Delay(TimeSpan.FromMilliseconds(100));
        this.ShowCustomPopupActivityIndicator(AddEditCategoryTypesContentPageResources.CustomPopupActivityIndicatorDeleteCategoryType);
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Log.Information("Attempt to delete category type : {Category}", json);
        await HandleCategoryTypeDelete(tCategory);
    }

    private void RefreshCategories()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        Categories.Clear();
        Categories.AddRange(context.VCategories.OrderBy(s => s.CategoryName));
    }

    private void RefreshCollection()
    {
        RefreshCategories();
        RefreshColors();
    }

    private void RefreshColors()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        Colors.Clear();
        Colors.AddRange(context.TColors.OrderBy(s => s.Name));
    }

    private void UpdateLanguage()
    {
        PlaceholderText = AddEditCategoryTypesContentPageResources.PlaceholderText;
        LabelTextColor = AddEditCategoryTypesContentPageResources.LabelTextColor;
        ButtonValidText = AddEditCategoryTypesContentPageResources.ButtonValidText;
    }

    private async Task<bool> ValidateCategoryType(string? categoryTypeName = null)
    {
        // ReSharper disable once HeapView.ClosureAllocation
        var categoryTypeNameToTest = string.IsNullOrWhiteSpace(categoryTypeName)
            ? CategoryTypeName
            : categoryTypeName;

        if (string.IsNullOrWhiteSpace(categoryTypeNameToTest))
        {
            await DisplayAlert(
                AddEditCategoryTypesContentPageResources.MessageBoxValidateCategoryTypeErrorEmptyTitle,
                AddEditCategoryTypesContentPageResources.MessageBoxValidateCategoryTypeErrorEmptyMessage,
                AddEditCategoryTypesContentPageResources.MessageBoxValidateCategoryTypeErrorEmptyOkButton);
            return false;
        }

        if (SelectedColor is null)
        {
            await DisplayAlert(
                AddEditCategoryTypesContentPageResources.MessageBoxValidateColorErrorEmptyTitle,
                AddEditCategoryTypesContentPageResources.MessageBoxValidateColorErrorEmptyMessage,
                AddEditCategoryTypesContentPageResources.MessageBoxValidateColorErrorEmptyOkButton);
            return false;
        }

        // ReSharper disable once HeapView.DelegateAllocation
        var alreadyExist = Categories.Any(s => s.CategoryName!.Equals(categoryTypeNameToTest));
        if (alreadyExist)
        {
            await DisplayAlert(
                AddEditCategoryTypesContentPageResources.MessageBoxValidateCategoryTypeErrorAlreadyExistTitle,
                AddEditCategoryTypesContentPageResources.MessageBoxValidateCategoryTypeErrorAlreadyExistMessage,
                AddEditCategoryTypesContentPageResources.MessageBoxValidateCategoryTypeErrorAlreadyExistOkButton);
            return false;
        }

        return true;
    }

    #endregion
}