using MyExpenses.Maui.Utils;

namespace MyExpenses.Smartphones.ContentPages;

public partial class DashBoardContentPage
{
    public static readonly BindableProperty LabelLocationProperty = BindableProperty.Create(nameof(LabelLocation),
        typeof(string), typeof(DashBoardContentPage), default(string));

    public DashBoardContentPage()
    {
        InitializeComponent();

        Task.Run(async () =>
        {
            while (true)
            {
                var location = await SensorRequestUtils.GetLocation();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var text = location is null
                        ? "Null"
                        : location.ToString();
                    LabelLocation = text;
                });

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        });
    }

    public string LabelLocation
    {
        get => (string)GetValue(LabelLocationProperty);
        set => SetValue(LabelLocationProperty, value);
    }
}