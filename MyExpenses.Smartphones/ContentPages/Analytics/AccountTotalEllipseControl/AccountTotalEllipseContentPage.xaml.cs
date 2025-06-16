namespace MyExpenses.Smartphones.ContentPages.Analytics.AccountTotalEllipseControl;

public partial class AccountTotalEllipseContentPage
{
    private readonly DeviceOrientationService _deviceOrientationService;

    public AccountTotalEllipseContentPage()
    {
        _deviceOrientationService = new DeviceOrientationService();

        InitializeComponent();
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