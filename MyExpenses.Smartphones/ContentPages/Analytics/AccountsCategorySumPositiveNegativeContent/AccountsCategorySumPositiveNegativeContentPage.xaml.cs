using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Ui;
using MyExpenses.SharedUtils.Resources.Resx.AnalyticsManagement;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages.Analytics.AccountsCategorySumPositiveNegativeContent;

public partial class AccountsCategorySumPositiveNegativeContentPage
{
    public static readonly BindableProperty AccountsCategorySumPositiveNegativeContentPageTitleProperty =
        BindableProperty.Create(nameof(AccountsCategorySumPositiveNegativeContentPageTitle), typeof(string),
            typeof(AccountsCategorySumPositiveNegativeContentPage));

    public string AccountsCategorySumPositiveNegativeContentPageTitle
    {
        get => (string)GetValue(AccountsCategorySumPositiveNegativeContentPageTitleProperty);
        set => SetValue(AccountsCategorySumPositiveNegativeContentPageTitleProperty, value);
    }

    public List<TabItemData> TabItemDatas { get; }

    private readonly DeviceOrientationService _deviceOrientationService;

    public AccountsCategorySumPositiveNegativeContentPage()
    {
        _deviceOrientationService = new DeviceOrientationService();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        TabItemDatas = [..context.TAccounts.OrderBy(s => s.Name)
            .Select(s => new TabItemData
            {
                Header = s.Name ?? string.Empty, Content = new AccountCategorySumPositiveNegativeContentView(), Id = s.Id
            })];

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += UpdateLanguage;
    }

    private void UpdateLanguage()
    {
        AccountsCategorySumPositiveNegativeContentPageTitle = AnalyticsManagementResources.TabItemAccountsCategorySumPositiveNegativeControl;
    }

    #region Action

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

    #endregion
}