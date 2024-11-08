using System.Collections.ObjectModel;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopupFilter;
using MyExpenses.Models.Sql.Derivatives.Views;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CustomPopups.CustomPopupFilterCategories;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupFilterCategories : ICustomPopupFilter<VCategoryDerive>
{
    public static readonly BindableProperty GeometrySourceProperty = BindableProperty.Create(nameof(GeometrySource),
        typeof(EPackIcons), typeof(CustomPopupFilterCategories), EPackIcons.CheckboxBlankOutline);

    public EPackIcons GeometrySource
    {
        get => (EPackIcons)GetValue(GeometrySourceProperty);
        set => SetValue(GeometrySourceProperty, value);
    }

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

    private string? SearchText { get; set; }

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

    private void CheckBox_OnCheckedChanged(object? sender, CheckedChangedEventArgs e)
        => CalculateCheckboxIconGeometrySource();

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void SearchBar_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        SearchText = e.NewTextValue;

        FilterCategoriesBySearchText();
    }

    private void SvgPath_OnClicked(object? sender, EventArgs e)
    {
        var check = GeometrySource is EPackIcons.CheckboxBlankOutline;

        OriginalCategories.ForEach(s => s.IsChecked = check);

        FilterCategoriesBySearchText();
        CalculateCheckboxIconGeometrySource();
    }

    #endregion

    #region Function

    private void CalculateCheckboxIconGeometrySource()
    {
        var allCategoriesCount = OriginalCategories.Count;
        var categoryDerivesCheckedCount = GetFilteredItemCheckedCount();

        EPackIcons icon;
        if (categoryDerivesCheckedCount is 0) icon = EPackIcons.CheckboxBlankOutline;
        else if (categoryDerivesCheckedCount.Equals(allCategoriesCount)) icon = EPackIcons.CheckboxOutline;
        else icon = EPackIcons.MinusCheckboxOutline;

        GeometrySource = icon;
    }

    private void FilterCategoriesBySearchText()
    {
        SearchText ??= string.Empty;

        var filterCategories = OriginalCategories.Where(s =>
            s.CategoryName!.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase));

        VCategoryDerives.Clear();
        VCategoryDerives.AddRange(filterCategories);
    }

    public IEnumerable<VCategoryDerive> GetFilteredItemChecked()
        => VCategoryDerives.Where(s => s.IsChecked);

    public int GetFilteredItemCheckedCount()
        => VCategoryDerives.Count(s => s.IsChecked);

    public int GetFilteredItemCount()
        => OriginalCategories.Count(s => s.IsChecked);

    private void UpdateLanguage()
    {
        SearchBarPlaceHolderText = CustomPopupFilterCategoriesResources.SearchBarPlaceHolderText;
        ButtonCloseText = CustomPopupFilterCategoriesResources.ButtonCloseText;
    }

    #endregion
}