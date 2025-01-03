using System.Collections.ObjectModel;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Derivatives.Tables;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CustomPopups.CustomPopupFilterModePayments;
using MyExpenses.Utils.Collection;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupFilterModePayments : ICustomPopupFilter<TModePaymentDerive>
{
    public static readonly BindableProperty GeometrySourceProperty = BindableProperty.Create(nameof(GeometrySource),
        typeof(EPackIcons), typeof(CustomPopupFilterModePayments), EPackIcons.CheckboxBlankOutline);

    public EPackIcons GeometrySource
    {
        get => (EPackIcons)GetValue(GeometrySourceProperty);
        set => SetValue(GeometrySourceProperty, value);
    }

    public static readonly BindableProperty SearchBarPlaceHolderTextProperty =
        BindableProperty.Create(nameof(SearchBarPlaceHolderText), typeof(string), typeof(CustomPopupFilterModePayments));

    public string SearchBarPlaceHolderText
    {
        get => (string)GetValue(SearchBarPlaceHolderTextProperty);
        set => SetValue(SearchBarPlaceHolderTextProperty, value);
    }

    public static readonly BindableProperty ButtonCloseTextProperty = BindableProperty.Create(nameof(ButtonCloseText),
        typeof(string), typeof(CustomPopupFilterModePayments));

    public string ButtonCloseText
    {
        get => (string)GetValue(ButtonCloseTextProperty);
        set => SetValue(ButtonCloseTextProperty, value);
    }

    private List<TModePaymentDerive> OriginalModePaymentDerives { get; }
    public ObservableCollection<TModePaymentDerive> ModePaymentDerives { get; }

    private string? SearchText { get; set; }

    public CustomPopupFilterModePayments(IReadOnlyCollection<TModePaymentDerive> currentTModePaymentDerives, IReadOnlyCollection<TModePaymentDerive>? modePaymentsAlreadyChecked = null)
    {
        OriginalModePaymentDerives = [..currentTModePaymentDerives];
        ModePaymentDerives = new ObservableCollection<TModePaymentDerive>(OriginalModePaymentDerives);

        if (modePaymentsAlreadyChecked is not null)
        {
            foreach (var modePaymentAlreadyChecked in modePaymentsAlreadyChecked.Where(s => s.IsChecked))
            {
                var modePaymentDerive = ModePaymentDerives.FirstOrDefault(s => s.Id.Equals(modePaymentAlreadyChecked.Id));
                if (modePaymentDerive is null) continue;
                modePaymentDerive.IsChecked = modePaymentAlreadyChecked.IsChecked;
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

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void SearchBar_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        SearchText = e.NewTextValue;

        FilterModePaymentBySearchText();
    }

    private void SvgPath_OnClicked(object? sender, EventArgs e)
    {
        var check = GeometrySource is EPackIcons.CheckboxBlankOutline;

        OriginalModePaymentDerives.ForEach(s => s.IsChecked = check);

        FilterModePaymentBySearchText();
        CalculateCheckboxIconGeometrySource();
    }

    #endregion

    #region Function

    private void CalculateCheckboxIconGeometrySource()
    {
        var allModePaymentsCount = OriginalModePaymentDerives.Count;
        var modePaymentDerivesCheckedCount = GetFilteredItemCheckedCount();

        EPackIcons icon;
        if (modePaymentDerivesCheckedCount is 0) icon = EPackIcons.CheckboxBlankOutline;
        else if (modePaymentDerivesCheckedCount.Equals(allModePaymentsCount)) icon = EPackIcons.CheckboxOutline;
        else icon = EPackIcons.MinusCheckboxOutline;

        GeometrySource = icon;
    }

    private void FilterModePaymentBySearchText()
    {
        SearchText ??= string.Empty;

        var filterCategories = OriginalModePaymentDerives.Where(s =>
            s.Name!.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase));

        ModePaymentDerives.Clear();
        ModePaymentDerives.AddRange(filterCategories);
    }

    public IEnumerable<TModePaymentDerive> GetFilteredItemChecked()
        => ModePaymentDerives.Where(s => s.IsChecked);

    public int GetFilteredItemCheckedCount()
        => ModePaymentDerives.Count(s => s.IsChecked);

    public int GetFilteredItemCount()
        => OriginalModePaymentDerives.Count;

    private void UpdateLanguage()
    {
        SearchBarPlaceHolderText = CustomPopupFilterModePaymentsResources.SearchBarPlaceHolderText;
        ButtonCloseText = CustomPopupFilterModePaymentsResources.ButtonCloseText;
    }

    #endregion
}