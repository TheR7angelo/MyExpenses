using System.Collections.ObjectModel;
using System.Windows.Input;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AccountTypeSummaryContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AddEditCategoryTypesContentPage
{
    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(AddEditCategoryTypesContentPage), default(string));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(AddEditCategoryTypesContentPage), default(string));

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
        typeof(TColor), typeof(AddEditCategoryTypesContentPage), default(TColor));

    public TColor? SelectedColor
    {
        get => (TColor)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public static readonly BindableProperty CategoryTypeNameProperty = BindableProperty.Create(nameof(CategoryTypeName),
        typeof(string), typeof(AddEditCategoryTypesContentPage), default(string));

    public string CategoryTypeName
    {
        get => (string)GetValue(CategoryTypeNameProperty);
        set => SetValue(CategoryTypeNameProperty, value);
    }

    public int MaxLength { get; } = 64;
    public ObservableCollection<TColor> Colors { get; } = [];
    public ObservableCollection<VCategory> Categories { get; } = [];

    public ICommand BackCommand { get; set; }

    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public AddEditCategoryTypesContentPage()
    {
        BackCommand = new Command(OnBackCommandPressed);

        RefreshCollection();

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        PlaceholderText = "PlaceholderText";
        ButtonValidText = "Valid";
    }

    private void RefreshCollection()
    {
        RefreshCategories();
        RefreshColors();
    }

    private void RefreshColors()
    {
        using var context = new DataBaseContext();
        Colors.Clear();
        Colors.AddRange(context.TColors.OrderBy(s => s.Name));
    }

    private void RefreshCategories()
    {
        using var context = new DataBaseContext();
        Categories.Clear();
        Categories.AddRange(context.VCategories.OrderBy(s => s.CategoryName));
    }

    private async void OnBackCommandPressed()
    {
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private async void ButtonValid_OnClicked(object? sender, EventArgs e)
    {
        var validate = await ValidateCategoryType();
        if (!validate) return;

        // TODO trad
        var response = await DisplayAlert(
            AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeQuestionTitle,
            string.Format(AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeQuestionMessage,
                CategoryTypeName),
            AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeQuestionYesButton,
            AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeQuestionNoButton);
        if (!response) return;

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

            await using var context = new DataBaseContext();
            var newVCategory = context.VCategories.First(s => s.Id.Equals(newCategoryTypeType.Id));
            Categories.AddAndSort(newVCategory, s => s.CategoryName!);

            CategoryTypeName = string.Empty;
            SelectedColor = null;

            // TODO trad
            await DisplayAlert(
                AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeSuccessTitle,
                AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeSuccessMessage,
                AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while adding new category type");
            // TODO trad
            await DisplayAlert(
                AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeErrorTitle,
                AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeErrorMessage,
                AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeErrorOkButton);
        }
    }

    private async Task<bool> ValidateCategoryType(string? categoryTypeName = null)
    {
        var categoryTypeNameToTest = string.IsNullOrWhiteSpace(categoryTypeName)
            ? CategoryTypeName
            : categoryTypeName;

        if (string.IsNullOrWhiteSpace(categoryTypeNameToTest))
        {
            // TODO trad
            await DisplayAlert(
                AccountTypeSummaryContentPageResources.MessageBoxValidateAccountTypeErrorEmptyTitle,
                AccountTypeSummaryContentPageResources.MessageBoxValidateAccountTypeErrorEmptyMessage,
                AccountTypeSummaryContentPageResources.MessageBoxValidateAccountTypeErrorEmptyOkButton);
            return false;
        }

        if (SelectedColor is null)
        {
            await DisplayAlert("Error", "Please select a color", "Ok");
            return false;
        }

        var alreadyExist = Categories.Any(s => s.CategoryName!.Equals(categoryTypeNameToTest));
        if (alreadyExist)
        {
            // TODO trad
            await DisplayAlert(
                AccountTypeSummaryContentPageResources.MessageBoxValidateAccountTypeErrorAlreadyExistTitle,
                AccountTypeSummaryContentPageResources.MessageBoxValidateAccountTypeErrorAlreadyExistMessage,
                AccountTypeSummaryContentPageResources.MessageBoxValidateAccountTypeErrorAlreadyExistOkButton);
            return false;
        }

        return true;
    }

    private void PickerColor_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        SelectedHexadecimalColorCode = SelectedColor is null
            ? "#00000000"
            : SelectedColor.HexadecimalColorCode!;
    }
}