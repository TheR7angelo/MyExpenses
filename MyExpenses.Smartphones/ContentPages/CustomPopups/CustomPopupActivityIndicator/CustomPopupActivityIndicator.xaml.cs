namespace MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;

public partial class CustomPopupActivityIndicator
{
    public static readonly BindableProperty LabelTextToDisplayProperty =
        BindableProperty.Create(nameof(LabelTextToDisplay), typeof(string), typeof(CustomPopupActivityIndicator),
            default(string));

    public string LabelTextToDisplay
    {
        get => (string)GetValue(LabelTextToDisplayProperty);
        set => SetValue(LabelTextToDisplayProperty, value);
    }

    public CustomPopupActivityIndicator()
    {
        InitializeComponent();
    }
}