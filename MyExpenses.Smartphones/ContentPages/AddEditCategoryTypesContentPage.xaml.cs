using System.Collections.ObjectModel;
using System.Windows.Input;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;

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

    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(nameof(SelectedColor),
        typeof(TColor), typeof(AddEditCategoryTypesContentPage), default(TColor));

    public TColor SelectedColor
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
}