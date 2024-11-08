using System.Collections.ObjectModel;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopupFilter;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Utils.Collection;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupFilterDescription : ICustomPopupFilter<StringIsChecked>
{
    public static readonly BindableProperty ButtonCloseTextProperty = BindableProperty.Create(nameof(ButtonCloseText),
        typeof(string), typeof(CustomPopupFilterDescription), default(string));

    public string ButtonCloseText
    {
        get => (string)GetValue(ButtonCloseTextProperty);
        set => SetValue(ButtonCloseTextProperty, value);
    }

    public static readonly BindableProperty SearchBarPlaceHolderTextProperty =
        BindableProperty.Create(nameof(SearchBarPlaceHolderText), typeof(string), typeof(CustomPopupFilterDescription),
            default(string));

    public string SearchBarPlaceHolderText
    {
        get => (string)GetValue(SearchBarPlaceHolderTextProperty);
        set => SetValue(SearchBarPlaceHolderTextProperty, value);
    }

    public static readonly BindableProperty GeometrySourceProperty = BindableProperty.Create(nameof(GeometrySource),
        typeof(EPackIcons), typeof(CustomPopupFilterDescription), EPackIcons.CheckboxBlankOutline);

    public EPackIcons GeometrySource
    {
        get => (EPackIcons)GetValue(GeometrySourceProperty);
        set => SetValue(GeometrySourceProperty, value);
    }

    private List<StringIsChecked> OriginalHistoryDescriptions { get; }
    public ObservableCollection<StringIsChecked> HistoryDescriptions { get; }

    private string? SearchText { get; set; }

    public CustomPopupFilterDescription(IEnumerable<StringIsChecked> currentHistoryDescriptions,
        IReadOnlyCollection<StringIsChecked>? historyDescriptionsAlreadyChecked = null)
    {
        OriginalHistoryDescriptions = [..currentHistoryDescriptions];
        HistoryDescriptions = new ObservableCollection<StringIsChecked>(OriginalHistoryDescriptions);

        if (historyDescriptionsAlreadyChecked is not null)
        {
            foreach (var historyDescriptionAlreadyChecked in historyDescriptionsAlreadyChecked.Where(s => s.IsChecked))
            {
                var histories = HistoryDescriptions.Where(s =>
                    s.StringValue!.Equals(historyDescriptionAlreadyChecked.StringValue)).ToList();
                if (histories.Count is 0) continue;
                histories.ForEach(s => s.IsChecked = historyDescriptionAlreadyChecked.IsChecked);
            }
        }

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        SearchBarPlaceHolderText = "SearchBar";
        ButtonCloseText = "Close";
    }

    public IEnumerable<StringIsChecked> GetFilteredItemChecked()
        => HistoryDescriptions.Where(s => s.IsChecked);

    public int GetFilteredItemCheckedCount()
        => HistoryDescriptions.Count(s => s.IsChecked);

    public int GetFilteredItemCount()
        => OriginalHistoryDescriptions.Count;

    private void SvgPath_OnClicked(object? sender, EventArgs e)
    {
        var check = GeometrySource is EPackIcons.CheckboxBlankOutline;

        OriginalHistoryDescriptions.ForEach(s => s.IsChecked = check);

        FilterHistoryDescriptionsBySearchText();
        CalculateCheckboxIconGeometrySource();
    }

    private void SearchBar_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        SearchText = e.NewTextValue;

        FilterHistoryDescriptionsBySearchText();
    }

    private void CheckBox_OnCheckedChanged(object? sender, CheckedChangedEventArgs e)
        => CalculateCheckboxIconGeometrySource();

    private void FilterHistoryDescriptionsBySearchText()
    {
        SearchText ??= string.Empty;

        var filterHistory = OriginalHistoryDescriptions.Where(s =>
            s.StringValue!.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase));

        HistoryDescriptions.Clear();
        HistoryDescriptions.AddRange(filterHistory);
    }

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

    private void ButtonClose_OnClicked(object? sender, EventArgs e)
        => Close();
}