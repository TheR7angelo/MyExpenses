using Foundation;
using UIKit;

namespace MyExpenses.Smartphones;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(255, 165, 0);

        return base.FinishedLaunching(app, options);
    }
}