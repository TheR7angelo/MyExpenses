using System.Collections.ObjectModel;
using System.Globalization;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CustomPopups.CustomPopupFilterHistoryValues;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Doubles;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupFilterDoubleValues : ICustomPopupFilter<DoubleIsChecked>
{
    public static readonly BindableProperty ButtonCloseTextProperty = BindableProperty.Create(nameof(ButtonCloseText),
        typeof(string), typeof(CustomPopupFilterDoubleValues));

    public string ButtonCloseText
    {
        get => (string)GetValue(ButtonCloseTextProperty);
        set => SetValue(ButtonCloseTextProperty, value);
    }

    public static readonly BindableProperty SearchBarPlaceHolderTextProperty =
        BindableProperty.Create(nameof(SearchBarPlaceHolderText), typeof(string), typeof(CustomPopupFilterDoubleValues));

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

    private List<DoubleIsChecked> OriginalHistoryValues { get; }
    public ObservableCollection<DoubleIsChecked> HistoryValues { get; }

    private string? SearchText { get; set; }

    public CustomPopupFilterDoubleValues(IEnumerable<DoubleIsChecked> currentHistoryValues,
        IReadOnlyCollection<DoubleIsChecked>? historyValuesAlreadyChecked = null)
    {
        OriginalHistoryValues = [..currentHistoryValues];
        HistoryValues = new ObservableCollection<DoubleIsChecked>(OriginalHistoryValues);

        if (historyValuesAlreadyChecked is not null)
        {
            foreach (var historyValueAlreadyChecked in historyValuesAlreadyChecked.Where(s => s.IsChecked))
            {
                var histories = HistoryValues
                    .Where(history => history.DoubleValue.HasValue &&
                                      historyValueAlreadyChecked.DoubleValue.HasValue &&
                                      history.DoubleValue.Value.AreEqual(historyValueAlreadyChecked.DoubleValue.Value))
                    .ToList();
                if (histories.Count is 0) continue;
                histories.ForEach(s => s.IsChecked = historyValueAlreadyChecked.IsChecked);
            }
        }

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonClose_OnClicked(object? sender, EventArgs e)
        => Close();

    private void CheckBox_OnCheckedChanged(object? sender, EventArgs eventArgs)
        => CalculateCheckboxIconGeometrySource();

    public IEnumerable<DoubleIsChecked> GetFilteredItemChecked()
        => HistoryValues.Where(s => s.IsChecked);

    public int GetFilteredItemCheckedCount()
        => HistoryValues.Count(s => s.IsChecked);

    public int GetFilteredItemCount()
        => OriginalHistoryValues.Count;

    private void SearchBar_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        SearchText = e.NewTextValue;

        FilterHistoryValuesBySearchText();
    }

    private void SvgPath_OnClicked(object? sender, EventArgs e)
    {
        var check = GeometrySource is EPackIcons.CheckboxBlankOutline;

        OriginalHistoryValues.ForEach(s => s.IsChecked = check);

        FilterHistoryValuesBySearchText();
        CalculateCheckboxIconGeometrySource();
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    private void CalculateCheckboxIconGeometrySource()
    {
        var allHistoryValuesCount = OriginalHistoryValues.Count;
        var historyValuesDerivesCheckedCount = GetFilteredItemCheckedCount();

        EPackIcons icon;
        if (historyValuesDerivesCheckedCount is 0) icon = EPackIcons.CheckboxBlankOutline;
        else if (historyValuesDerivesCheckedCount.Equals(allHistoryValuesCount)) icon = EPackIcons.CheckboxOutline;
        else icon = EPackIcons.MinusCheckboxOutline;

        GeometrySource = icon;
    }

    private void FilterHistoryValuesBySearchText()
    {
        SearchText ??= "0";
        var value = SearchText.Replace(',', '.');
        var result = double.TryParse(value, out var parsedResult) ? parsedResult : 0;

        var filterHistory = OriginalHistoryValues.Where(s =>
            s.DoubleValue!.ToString()!.Contains(result.ToString(CultureInfo.InvariantCulture)));

        HistoryValues.Clear();
        HistoryValues.AddRange(filterHistory);
    }

    private void UpdateLanguage()
    {
        SearchBarPlaceHolderText = CustomPopupFilterDoubleValuesResources.SearchBarPlaceHolderText;
        ButtonCloseText = CustomPopupFilterDoubleValuesResources.ButtonCloseText;
    }

    #endregion
}