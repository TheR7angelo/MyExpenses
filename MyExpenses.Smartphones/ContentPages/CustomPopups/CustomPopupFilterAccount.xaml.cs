using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Derivatives.Tables;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CustomPopups.CustomPopupFilterAccount;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupFilterAccount : ICustomPopupFilter<TAccountDerive>
{
    public static readonly BindableProperty ButtonCloseTextProperty = BindableProperty.Create(nameof(ButtonCloseText),
        typeof(string), typeof(CustomPopupFilterAccount));

    public string ButtonCloseText
    {
        get => (string)GetValue(ButtonCloseTextProperty);
        set => SetValue(ButtonCloseTextProperty, value);
    }

    public static readonly BindableProperty SearchBarPlaceHolderTextProperty =
        BindableProperty.Create(nameof(SearchBarPlaceHolderText), typeof(string), typeof(CustomPopupFilterAccount));

    public string SearchBarPlaceHolderText
    {
        get => (string)GetValue(SearchBarPlaceHolderTextProperty);
        set => SetValue(SearchBarPlaceHolderTextProperty, value);
    }

    public static readonly BindableProperty GeometrySourceProperty = BindableProperty.Create(nameof(GeometrySource),
        // ReSharper disable once HeapView.BoxingAllocation
        typeof(EPackIcons), typeof(CustomPopupFilterAccount), EPackIcons.CheckboxBlankOutline);

    public EPackIcons GeometrySource
    {
        get => (EPackIcons)GetValue(GeometrySourceProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(GeometrySourceProperty, value);
    }

    private List<TAccountDerive> OriginalAccountDerives { get; }
    public List<TAccountDerive> AccountDerives { get; }

    private string? SearchText { get; set; }

    public CustomPopupFilterAccount(IEnumerable<TAccountDerive> currentAccountDerives,
        IReadOnlyCollection<TAccountDerive>? accountDerivesAlreadyChecked = null)
    {
        OriginalAccountDerives = [..currentAccountDerives];
        AccountDerives = [..OriginalAccountDerives];

        if (accountDerivesAlreadyChecked is not null)
        {
            // ReSharper disable once HeapView.ClosureAllocation
            foreach (var accountDeriveAlreadyChecked in accountDerivesAlreadyChecked.Where(s => s.IsChecked).ToArray())
            {
                // ReSharper disable once HeapView.DelegateAllocation
                var histories = AccountDerives.Where(s => s.Name!.Equals(accountDeriveAlreadyChecked.Name)).ToList();
                if (histories.Count is 0) continue;

                foreach (var history in histories)
                {
                    history.IsChecked = accountDeriveAlreadyChecked.IsChecked;
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

    public IEnumerable<TAccountDerive> GetFilteredItemChecked()
        => AccountDerives.Where(s => s.IsChecked);

    public int GetFilteredItemCheckedCount()
        => AccountDerives.Count(s => s.IsChecked);

    public int GetFilteredItemCount()
        => OriginalAccountDerives.Count;

    private void SearchBar_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        SearchText = e.NewTextValue;

        FilterAccountNamesBySearchText();
    }

    private void SvgPath_OnClicked(object? sender, EventArgs e)
    {
        var check = GeometrySource is EPackIcons.CheckboxBlankOutline;

        foreach (var originalAccountDerive in OriginalAccountDerives)
        {
            originalAccountDerive.IsChecked = check;
        }

        FilterAccountNamesBySearchText();
        CalculateCheckboxIconGeometrySource();
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    private void CalculateCheckboxIconGeometrySource()
    {
        var allAccountCount = OriginalAccountDerives.Count;
        var accountDerivesCheckedCount = GetFilteredItemCheckedCount();

        EPackIcons icon;
        if (accountDerivesCheckedCount is 0) icon = EPackIcons.CheckboxBlankOutline;
        else if (accountDerivesCheckedCount.Equals(allAccountCount)) icon = EPackIcons.CheckboxOutline;
        else icon = EPackIcons.MinusCheckboxOutline;

        GeometrySource = icon;
    }

    private void FilterAccountNamesBySearchText()
    {
        SearchText ??= string.Empty;

        // ReSharper disable once HeapView.DelegateAllocation
        var filterAccountNames = OriginalAccountDerives.Where(s => s.Name!.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase));

        AccountDerives.Clear();
        AccountDerives.AddRange(filterAccountNames);
    }

    private void UpdateLanguage()
    {
        SearchBarPlaceHolderText = CustomPopupFilterAccountResources.SearchBarPlaceHolderText;
        ButtonCloseText = CustomPopupFilterAccountResources.ButtonCloseText;
    }

    #endregion
}