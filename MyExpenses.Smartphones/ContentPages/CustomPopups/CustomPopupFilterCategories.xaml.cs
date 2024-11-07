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

    public CustomPopupFilterCategories(IReadOnlyCollection<VCategoryDerive>? categoryDerivesAlreadyChecked = null)
    {
        var mapper = Mapping.Mapper;

        using var context = new DataBaseContext();
        VCategoryDerives = [..context.VCategories.Select(s => mapper.Map<VCategoryDerive>(s))];

        if (categoryDerivesAlreadyChecked is not null)
        {
            foreach (var categoryDeriveAlreadyChecked in categoryDerivesAlreadyChecked.Where(s => s.IsChecked))
            {
                var categoryDerive = VCategoryDerives.FirstOrDefault(s => s.Id == categoryDeriveAlreadyChecked.Id);
                if (categoryDerive is null) continue;
                categoryDerive.IsChecked = categoryDeriveAlreadyChecked.IsChecked;
            }
        }

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonClose_OnClicked(object? sender, EventArgs e)
        => Close();

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void SearchBar_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        //TODO work
    }

    #endregion

    #region Function

    public ImmutableList<VCategoryDerive> GetVCategoryDerivesChecked()
        => VCategoryDerives.Where(s => s.IsChecked).ToImmutableList();

    public int GetVCategoryDerivesCheckedCount()
        => GetVCategoryDerivesChecked().Count;

    private void UpdateLanguage()
    {
        ButtonCloseText = CustomPopupFilterCategoriesResources.ButtonCloseText;
    }

    #endregion
}