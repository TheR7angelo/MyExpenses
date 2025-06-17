using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Ui;
using MyExpenses.SharedUtils.Resources.Resx.AnalyticsManagement;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages.Analytics.AccountTotalEllipseContent;

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

    public List<TabItemData> TabItemDatas { get; }

    public AccountTotalEllipseContentPage()
    {
        _deviceOrientationService = new DeviceOrientationService();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        using var context = new DataBaseContext();
        TabItemDatas = [..context.VTotalByAccounts.OrderBy(s => s.Name)
            .Select(s => new TabItemData { Header = s.Name!, Content = s })];

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