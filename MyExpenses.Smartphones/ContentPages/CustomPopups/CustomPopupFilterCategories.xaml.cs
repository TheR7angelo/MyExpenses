using System.Collections.Immutable;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Derivatives.Views;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CustomPopups.CustomPopupFilterCategories;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupFilterCategories
{
    public static readonly BindableProperty ButtonCloseTextProperty = BindableProperty.Create(nameof(ButtonCloseText),
        typeof(string), typeof(CustomPopupFilterCategories), default(string));

    public string ButtonCloseText
    {
        get => (string)GetValue(ButtonCloseTextProperty);
        set => SetValue(ButtonCloseTextProperty, value);
    }

    public List<VCategoryDerive> VCategoryDerives { get; }

    public CustomPopupFilterCategories()
    {
        var mapper = Mapping.Mapper;

        using var context = new DataBaseContext();
        VCategoryDerives = [..context.VCategories.Select(s => mapper.Map<VCategoryDerive>(s))];

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        ButtonCloseText = CustomPopupFilterCategoriesResources.ButtonCloseText;
    }


    public ImmutableList<VCategoryDerive> GetVCategoryDerivesChecked()
        => VCategoryDerives.Where(s => s.IsChecked).ToImmutableList();

    public int GetVCategoryDerivesCheckedCount()
        => GetVCategoryDerivesChecked().Count;

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        Close();
    }

    private void SearchBar_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        //TODO work
    }
}