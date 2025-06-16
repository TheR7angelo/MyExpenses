using System.Collections.ObjectModel;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Derivatives.Views;
using MyExpenses.SharedUtils.Resources.Resx.AnalyticsManagement;
using MyExpenses.Sql.Context;

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

    public ObservableCollection<VTotalByAccountAnalyse> VTotalByAccountAnalyses { get; }

    public AccountTotalEllipseContentPage()
    {
        _deviceOrientationService = new DeviceOrientationService();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        using var context = new DataBaseContext();
        VTotalByAccountAnalyses = [..context.VTotalByAccounts.OrderBy(s => s.Name)
            .Select(s => Mapping.Mapper.Map<VTotalByAccountAnalyse>(s))];

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