using MyExpenses.Models.Config.Interfaces;
using MyExpenses.SharedUtils.Resources.Resx.GeneralAccountManagement;

namespace MyExpenses.Smartphones.ContentPages;

public partial class GeneralAccountSetupContentPage
{
    public static readonly BindableProperty ButtonTextPaymentMethodManagementProperty =
        BindableProperty.Create(nameof(ButtonTextPaymentMethodManagement), typeof(string),
            typeof(GeneralAccountSetupContentPage));

    public string ButtonTextPaymentMethodManagement
    {
        get => (string)GetValue(ButtonTextPaymentMethodManagementProperty);
        set => SetValue(ButtonTextPaymentMethodManagementProperty, value);
    }
    
    public static readonly BindableProperty ButtonTextCurrencySymbolManagementProperty =
        BindableProperty.Create(nameof(ButtonTextCurrencySymbolManagement), typeof(string),
            typeof(GeneralAccountSetupContentPage));

    public string ButtonTextCurrencySymbolManagement
    {
        get => (string)GetValue(ButtonTextCurrencySymbolManagementProperty);
        set => SetValue(ButtonTextCurrencySymbolManagementProperty, value);
    }

    public static readonly BindableProperty ButtonTextColorManagementProperty =
        BindableProperty.Create(nameof(ButtonTextColorManagement), typeof(string),
            typeof(GeneralAccountSetupContentPage));

    public string ButtonTextColorManagement
    {
        get => (string)GetValue(ButtonTextColorManagementProperty);
        set => SetValue(ButtonTextColorManagementProperty, value);
    }

    public static readonly BindableProperty ButtonTextLocationManagementProperty =
        BindableProperty.Create(nameof(ButtonTextLocationManagement), typeof(string),
            typeof(GeneralAccountSetupContentPage));

    public string ButtonTextLocationManagement
    {
        get => (string)GetValue(ButtonTextLocationManagementProperty);
        set => SetValue(ButtonTextLocationManagementProperty, value);
    }

    public static readonly BindableProperty ButtonTextManagingCategoryTypesProperty =
        BindableProperty.Create(nameof(ButtonTextManagingCategoryTypes), typeof(string),
            typeof(GeneralAccountSetupContentPage));

    public string ButtonTextManagingCategoryTypes
    {
        get => (string)GetValue(ButtonTextManagingCategoryTypesProperty);
        set => SetValue(ButtonTextManagingCategoryTypesProperty, value);
    }

    public static readonly BindableProperty ButtonTextManagingAccountTypesProperty =
        BindableProperty.Create(nameof(ButtonTextManagingAccountTypes), typeof(string),
            typeof(GeneralAccountSetupContentPage));

    public string ButtonTextManagingAccountTypes
    {
        get => (string)GetValue(ButtonTextManagingAccountTypesProperty);
        set => SetValue(ButtonTextManagingAccountTypesProperty, value);
    }

    public static readonly BindableProperty ButtonTextAccountManagementProperty =
        BindableProperty.Create(nameof(ButtonTextAccountManagement), typeof(string),
            typeof(GeneralAccountSetupContentPage));

    public string ButtonTextAccountManagement
    {
        get => (string)GetValue(ButtonTextAccountManagementProperty);
        set => SetValue(ButtonTextAccountManagementProperty, value);
    }

    public GeneralAccountSetupContentPage()
    {
        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonAccountManagement_OnClicked(object? sender, EventArgs e)
        => _ = typeof(AccountManagementContentPage).NavigateToAsync();

    private void ButtonColorManagement_OnClicked(object? sender, EventArgs e)
        => _ = typeof(ColorManagementContentPage).NavigateToAsync();

    private void ButtonCurrencyManagement_OnClicked(object? sender, EventArgs e)
        => _ = typeof(CurrencyManagementContentPage).NavigateToAsync();

    private void ButtonLocationManagement_OnClicked(object? sender, EventArgs e)
        => _ = typeof(LocationManagementContentPage).NavigateToAsync();

    private void ButtonManagingAccountTypes_OnClicked(object? sender, EventArgs e)
        => _ = typeof(AccountTypeSummaryContentPage).NavigateToAsync();

    private void ButtonManagingCategoryTypes_OnClicked(object? sender, EventArgs e)
        => _ = typeof(AddEditCategoryTypesContentPage).NavigateToAsync();

    private void ButtonModePaymentManagement_OnClicked(object? sender, EventArgs e)
        => _ = typeof(ModePaymentManagementContentPage).NavigateToAsync();

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    private void UpdateLanguage()
    {
        ButtonTextAccountManagement = GeneralAccountManagementResources.ButtonTextAccountManagement;
        ButtonTextManagingAccountTypes = GeneralAccountManagementResources.ButtonTextManagingAccountTypes;
        ButtonTextManagingCategoryTypes = GeneralAccountManagementResources.ButtonTextManagingCategoryTypes;
        ButtonTextLocationManagement = GeneralAccountManagementResources.ButtonTextLocationManagement;
        ButtonTextColorManagement = GeneralAccountManagementResources.ButtonTextColorManagement;
        ButtonTextCurrencySymbolManagement = GeneralAccountManagementResources.ButtonTextCurrencySymbolManagement;
        ButtonTextPaymentMethodManagement = GeneralAccountManagementResources.ButtonTextPaymentMethodManagement;
    }

    #endregion
}