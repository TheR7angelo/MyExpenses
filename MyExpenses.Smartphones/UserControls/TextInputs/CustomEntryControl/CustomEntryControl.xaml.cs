namespace MyExpenses.Smartphones.UserControls.TextInputs.CustomEntryControl;

public partial class CustomEntryControl
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomEntryControl), null);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty =
        BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(CustomEntryControl), string.Empty);

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty IsPlaceholderVisibleProperty =
        BindableProperty.Create(nameof(IsPlaceholderVisible), typeof(bool), typeof(CustomEntryControl), true);

    public bool IsPlaceholderVisible
    {
        get => (bool)GetValue(IsPlaceholderVisibleProperty);
        set => SetValue(IsPlaceholderVisibleProperty, value);
    }

    public CustomEntryControl()
    {
        InitializeComponent();
    }

    private void InputView_OnTextChanged(object? sender, TextChangedEventArgs e)
        => IsPlaceholderVisible = string.IsNullOrEmpty(e.NewTextValue);

}