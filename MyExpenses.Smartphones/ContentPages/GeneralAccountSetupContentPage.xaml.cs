using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.GeneralAccountSetupContentPage;

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
        => _ = NavigateTo(typeof(AccountManagementContentPage));

    private void ButtonLocationManagement_OnClicked(object? sender, EventArgs e)
        => _ = NavigateTo(typeof(LocationManagementContentPage));

    private void ButtonManagingAccountTypes_OnClicked(object? sender, EventArgs e)
        => _ = NavigateTo(typeof(AccountTypeSummaryContentPage));

    private void ButtonManagingCategoryTypes_OnClicked(object? sender, EventArgs e)
        => _ = NavigateTo(typeof(AddEditCategoryTypesContentPage));

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    private async Task NavigateTo(Type type)
    {
        var contentPage = (ContentPage)Activator.CreateInstance(type)!;
        await Navigation.PushAsync(contentPage);
    }

    private void UpdateLanguage()
    {
        ButtonTextAccountManagement = GeneralAccountSetupContentPageResources.ButtonTextAccountManagement;
        ButtonTextManagingAccountTypes = GeneralAccountSetupContentPageResources.ButtonTextManagingAccountTypes;
        ButtonTextManagingCategoryTypes = GeneralAccountSetupContentPageResources.ButtonTextManagingCategoryTypes;
        ButtonTextLocationManagement = GeneralAccountSetupContentPageResources.ButtonTextLocationManagement;
        ButtonTextColorManagement = GeneralAccountSetupContentPageResources.ButtonTextColorManagement;
        ButtonTextCurrencySymbolManagement = GeneralAccountSetupContentPageResources.ButtonTextCurrencySymbolManagement;
        ButtonTextPaymentMethodManagement = GeneralAccountSetupContentPageResources.ButtonTextPaymentMethodManagement;
    }

    #endregion
}