using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.SharedUtils.Resources.Resx.PopupFilterManagement;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Utils.PopupFilter;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class PopupFilter
{
    public static readonly BindableProperty ButtonCloseTextProperty = BindableProperty.Create(nameof(ButtonCloseText),
        typeof(string), typeof(PopupFilter));

    public string ButtonCloseText
    {
        get => (string)GetValue(ButtonCloseTextProperty);
        set => SetValue(ButtonCloseTextProperty, value);
    }

    public static readonly BindableProperty SearchBarPlaceHolderTextProperty =
        BindableProperty.Create(nameof(SearchBarPlaceHolderText), typeof(string), typeof(PopupFilter));

    public string SearchBarPlaceHolderText
    {
        get => (string)GetValue(SearchBarPlaceHolderTextProperty);
        set => SetValue(SearchBarPlaceHolderTextProperty, value);
    }

    public static readonly BindableProperty GeometrySourceProperty = BindableProperty.Create(nameof(GeometrySource),
        // ReSharper disable once HeapView.BoxingAllocation
        typeof(EPackIcons), typeof(PopupFilter), EPackIcons.CheckboxBlankOutline);

    public EPackIcons GeometrySource
    {
        get => (EPackIcons)GetValue(GeometrySourceProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(GeometrySourceProperty, value);
    }

    private List<PopupSearch> OriginalPopupSearches { get; }
    public List<PopupSearch> PopupSearches { get; }

    private string? SearchText { get; set; }

    private EPopupSearch EPopupSearch { get; }

    public PopupFilter(IEnumerable<PopupSearch> currentPopupSearches, EPopupSearch ePopupSearch,
        IReadOnlyCollection<PopupSearch>? currentPopupSearchesAlreadyChecked = null)
    {
        EPopupSearch = ePopupSearch;

        OriginalPopupSearches = [..currentPopupSearches];
        PopupSearches = [..OriginalPopupSearches];

        if (currentPopupSearchesAlreadyChecked is not null)
        {
            // ReSharper disable once HeapView.ClosureAllocation
            foreach (var currentPopupSearchAlreadyChecked in currentPopupSearchesAlreadyChecked.Where(s => s.IsChecked).ToArray())
            {
                // ReSharper disable once HeapView.DelegateAllocation
                var popupSearches = PopupSearches.Where(s => s.Content.Equals(currentPopupSearchAlreadyChecked.Content)).ToList();

                if (popupSearches.Count is 0) continue;

                foreach (var popupSearch in popupSearches)
                {
                    popupSearch.IsChecked = currentPopupSearchAlreadyChecked.IsChecked;
                }
            }
        }

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonClose_OnClicked(object? sender, EventArgs e)
        => Close();

    private void CheckBox_OnCheckedChanged(object? sender, EventArgs eventArgs)
        => CalculateCheckboxIconGeometrySource();

    public IEnumerable<PopupSearch> GetFilteredItemChecked()
        => PopupSearches.Where(s => s.IsChecked);

    public IEnumerable<T> GetFilteredItemChecked<T>()
    {
        // ReSharper disable once HeapView.ClosureAllocation
        return GetFilteredItemChecked().ToTable<T>()!;
    }

    public int GetFilteredItemCheckedCount()
        => PopupSearches.Count(s => s.IsChecked);

    public int GetFilteredItemCount()
        => OriginalPopupSearches.Count;

    private void SearchBar_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        SearchText = e.NewTextValue;

        FilterPopupSearchesBySearchText();
    }

    private void SvgPath_OnClicked(object? sender, EventArgs e)
    {
        var check = GeometrySource is EPackIcons.CheckboxBlankOutline;

        foreach (var originalAccountDerive in OriginalPopupSearches)
        {
            originalAccountDerive.IsChecked = check;
        }

        FilterPopupSearchesBySearchText();
        CalculateCheckboxIconGeometrySource();
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    private void CalculateCheckboxIconGeometrySource()
    {
        var allAccountCount = OriginalPopupSearches.Count;
        var filteredItemCheckedCount = GetFilteredItemCheckedCount();

        EPackIcons icon;
        if (filteredItemCheckedCount is 0) icon = EPackIcons.CheckboxBlankOutline;
        else if (filteredItemCheckedCount.Equals(allAccountCount)) icon = EPackIcons.CheckboxOutline;
        else icon = EPackIcons.MinusCheckboxOutline;

        GeometrySource = icon;
    }

    private void FilterPopupSearchesBySearchText()
    {
        SearchText ??= string.Empty;

        // ReSharper disable once HeapView.DelegateAllocation
        var filterAccountNames = OriginalPopupSearches.Where(s => s.Content.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase));

        PopupSearches.Clear();
        PopupSearches.AddRange(filterAccountNames);
    }

    private void UpdateLanguage()
    {
        SearchBarPlaceHolderText = EPopupSearch switch
        {
            EPopupSearch.Account => PopupFilterManagementResources.SearchBarPlaceHolderTextAccount,
            _ => throw new ArgumentOutOfRangeException()
        };

        ButtonCloseText = PopupFilterManagementResources.ButtonCloseText;
    }

    #endregion
}