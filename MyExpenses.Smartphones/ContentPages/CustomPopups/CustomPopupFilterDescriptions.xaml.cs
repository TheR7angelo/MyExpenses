using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CustomPopups.CustomPopupFilterHistoryDescriptions;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupFilterDescriptions : ICustomPopupFilter<StringIsChecked>
{
    public static readonly BindableProperty ButtonCloseTextProperty = BindableProperty.Create(nameof(ButtonCloseText),
        typeof(string), typeof(CustomPopupFilterDescriptions));

    public string ButtonCloseText
    {
        get => (string)GetValue(ButtonCloseTextProperty);
        set => SetValue(ButtonCloseTextProperty, value);
    }

    public static readonly BindableProperty SearchBarPlaceHolderTextProperty =
        BindableProperty.Create(nameof(SearchBarPlaceHolderText), typeof(string), typeof(CustomPopupFilterDescriptions));

    public string SearchBarPlaceHolderText
    {
        get => (string)GetValue(SearchBarPlaceHolderTextProperty);
        set => SetValue(SearchBarPlaceHolderTextProperty, value);
    }

    public static readonly BindableProperty GeometrySourceProperty = BindableProperty.Create(nameof(GeometrySource),
        // ReSharper disable once HeapView.BoxingAllocation
        typeof(EPackIcons), typeof(CustomPopupFilterDescriptions), EPackIcons.CheckboxBlankOutline);

    public EPackIcons GeometrySource
    {
        get => (EPackIcons)GetValue(GeometrySourceProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(GeometrySourceProperty, value);
    }

    private List<StringIsChecked> OriginalHistoryDescriptions { get; }
    public List<StringIsChecked> HistoryDescriptions { get; }

    private string? SearchText { get; set; }

    public CustomPopupFilterDescriptions(IEnumerable<StringIsChecked> currentHistoryDescriptions,
        IReadOnlyCollection<StringIsChecked>? historyDescriptionsAlreadyChecked = null)
    {
        OriginalHistoryDescriptions = [..currentHistoryDescriptions];
        HistoryDescriptions = [..OriginalHistoryDescriptions];

        if (historyDescriptionsAlreadyChecked is not null)
        {
            // ReSharper disable once HeapView.ClosureAllocation
            foreach (var historyDescriptionAlreadyChecked in historyDescriptionsAlreadyChecked.Where(s => s.IsChecked).ToArray())
            {
                // ReSharper disable once HeapView.DelegateAllocation
                var histories = HistoryDescriptions.Where(s => s.StringValue == historyDescriptionAlreadyChecked.StringValue).ToList();
                if (histories.Count is 0) continue;

                foreach (var history in histories)
                {
                    history.IsChecked = historyDescriptionAlreadyChecked.IsChecked;
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

    public IEnumerable<StringIsChecked> GetFilteredItemChecked()
        => HistoryDescriptions.Where(s => s.IsChecked);

    public int GetFilteredItemCheckedCount()
        => HistoryDescriptions.Count(s => s.IsChecked);

    public int GetFilteredItemCount()
        => OriginalHistoryDescriptions.Count;

    private void SearchBar_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        SearchText = e.NewTextValue;

        FilterHistoryDescriptionsBySearchText();
    }

    private void SvgPath_OnClicked(object? sender, EventArgs e)
    {
        var check = GeometrySource is EPackIcons.CheckboxBlankOutline;

        foreach (var originalHistoryDescription in OriginalHistoryDescriptions)
        {
            originalHistoryDescription.IsChecked = check;
        }

        FilterHistoryDescriptionsBySearchText();
        CalculateCheckboxIconGeometrySource();
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    private void CalculateCheckboxIconGeometrySource()
    {
        var allCategoriesCount = OriginalHistoryDescriptions.Count;
        var categoryDerivesCheckedCount = GetFilteredItemCheckedCount();

        EPackIcons icon;
        if (categoryDerivesCheckedCount is 0) icon = EPackIcons.CheckboxBlankOutline;
        else if (categoryDerivesCheckedCount.Equals(allCategoriesCount)) icon = EPackIcons.CheckboxOutline;
        else icon = EPackIcons.MinusCheckboxOutline;

        GeometrySource = icon;
    }

    private void FilterHistoryDescriptionsBySearchText()
    {
        SearchText ??= string.Empty;

        // ReSharper disable once HeapView.DelegateAllocation
        var filterHistory = OriginalHistoryDescriptions.Where(s => s.StringValue!.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase));

        HistoryDescriptions.Clear();
        HistoryDescriptions.AddRange(filterHistory);
    }

    private void UpdateLanguage()
    {
        SearchBarPlaceHolderText = CustomPopupFilterDescriptionsResources.SearchBarPlaceHolderText;
        ButtonCloseText = CustomPopupFilterDescriptionsResources.ButtonCloseText;
    }

    #endregion
}