using Android.App;
using Android.Content.PM;

namespace MyExpenses.Smartphones;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(
    new[] { Android.Content.Intent.ActionView },
    Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
    DataSchemes = new[] { "myexpenses" },
    DataHost = "redirect"
)]
public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
{
}