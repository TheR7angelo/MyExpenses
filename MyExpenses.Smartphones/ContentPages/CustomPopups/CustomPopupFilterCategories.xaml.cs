using System.Collections.Immutable;
using System.Collections.ObjectModel;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Derivatives.Views;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CustomPopups.CustomPopupFilterCategories;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupFilterCategories
{
    public static readonly BindableProperty SearchBarPlaceHolderTextProperty =
        BindableProperty.Create(nameof(SearchBarPlaceHolderText), typeof(string), typeof(CustomPopupFilterCategories),
            default(string));

    public string SearchBarPlaceHolderText
    {
        get => (string)GetValue(SearchBarPlaceHolderTextProperty);
        set => SetValue(SearchBarPlaceHolderTextProperty, value);
    }

    public static readonly BindableProperty ButtonCloseTextProperty = BindableProperty.Create(nameof(ButtonCloseText),
        typeof(string), typeof(CustomPopupFilterCategories), default(string));

    public string ButtonCloseText
    {
        get => (string)GetValue(ButtonCloseTextProperty);
        set => SetValue(ButtonCloseTextProperty, value);
    }

    private List<VCategoryDerive> OriginalCategories { get; }
    public ObservableCollection<VCategoryDerive> VCategoryDerives { get; }

    public CustomPopupFilterCategories(IReadOnlyCollection<VCategoryDerive>? categoryDerivesAlreadyChecked = null)
    {
        var mapper = Mapping.Mapper;

        using var context = new DataBaseContext();
        OriginalCategories = [..context.VCategories.OrderBy(s => s.CategoryName).Select(s => mapper.Map<VCategoryDerive>(s))];
        VCategoryDerives = new ObservableCollection<VCategoryDerive>(OriginalCategories);

        if (categoryDerivesAlreadyChecked is not null)
        {
            foreach (var categoryDeriveAlreadyChecked in categoryDerivesAlreadyChecked.Where(s => s.IsChecked))
            {
                var categoryDerive = VCategoryDerives.FirstOrDefault(s => s.Id.Equals(categoryDeriveAlreadyChecked.Id));
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
        var text = e.NewTextValue;

        var filterCategories = OriginalCategories.Where(s =>
            s.CategoryName!.Contains(text, StringComparison.InvariantCultureIgnoreCase));

        VCategoryDerives.Clear();
        VCategoryDerives.AddRange(filterCategories);
    }

    #endregion

    #region Function

    public IEnumerable<VCategoryDerive> GetVCategoryDerivesChecked()
        => VCategoryDerives.Where(s => s.IsChecked);

    public int GetVCategoryDerivesCheckedCount()
        => VCategoryDerives.Count(s => s.IsChecked);

    private void UpdateLanguage()
    {
        SearchBarPlaceHolderText = CustomPopupFilterCategoriesResources.SearchBarPlaceHolderText;
        ButtonCloseText = CustomPopupFilterCategoriesResources.ButtonCloseText;
    }

    #endregion
}