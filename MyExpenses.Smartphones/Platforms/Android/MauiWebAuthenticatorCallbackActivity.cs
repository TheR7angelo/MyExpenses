using Android.App;
using Android.Content.PM;

namespace MyExpenses.Smartphones;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter([Android.Content.Intent.ActionView],
    Categories = [Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable],
    DataSchemes = ["http"])]
public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
{
}