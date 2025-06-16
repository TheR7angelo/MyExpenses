using MyExpenses.Models.Config.Interfaces;
using MyExpenses.SharedUtils.Resources.Resx.AnalyticsManagement;

namespace MyExpenses.Smartphones.ContentPages.Analytics.AccountTotalEllipseControl;

public partial class AccountTotalEllipseContentPage
{
    public static readonly BindableProperty AccountTotalEllipseContentPageTitleProperty =
        BindableProperty.Create(nameof(AccountTotalEllipseContentPageTitle), typeof(string),
            typeof(AccountTotalEllipseContentPage));

    public string AccountTotalEllipseContentPageTitle
    {
        get => (string)GetValue(AccountTotalEllipseContentPageTitleProperty);
        set => SetValue(AccountTotalEllipseContentPageTitleProperty, value);
    }

    private readonly DeviceOrientationService _deviceOrientationService;

    public AccountTotalEllipseContentPage()
    {
        _deviceOrientationService = new DeviceOrientationService();

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += UpdateLanguage;
    }

    private void UpdateLanguage()
    {
        AccountTotalEllipseContentPageTitle = AnalyticsManagementResources.TabItemAccountTotalEllipseControlHeader;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _deviceOrientationService.SetDeviceOrientation(DisplayOrientation.Landscape);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _deviceOrientationService.SetDeviceOrientation(DisplayOrientation.Unknown);
    }
}