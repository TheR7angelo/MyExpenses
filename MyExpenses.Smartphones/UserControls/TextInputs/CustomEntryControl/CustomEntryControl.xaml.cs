using MyExpenses.Smartphones.PackIcons;

namespace MyExpenses.Smartphones.UserControls.TextInputs.CustomEntryControl;

public partial class CustomEntryControl
{
    public static readonly BindableProperty MaxLengthProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(CustomEntryControl), 255);

    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(MaxLengthProperty, value);
    }

    public static readonly BindableProperty HasMultilineProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(HasMultiline), typeof(bool), typeof(CustomEntryControl), false);

    public bool HasMultiline
    {
        get => (bool)GetValue(HasMultilineProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(HasMultilineProperty, value);
    }

    public static readonly BindableProperty HasClearButtonProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(HasClearButton), typeof(bool), typeof(CustomEntryControl), false);

    public bool HasClearButton
    {
        get => (bool)GetValue(HasClearButtonProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(HasClearButtonProperty, value);
    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomEntryControl));

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
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(IsPlaceholderVisible), typeof(bool), typeof(CustomEntryControl), true);

    public bool IsPlaceholderVisible
    {
        get => (bool)GetValue(IsPlaceholderVisibleProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(IsPlaceholderVisibleProperty, value);
    }

    public EPackIcons CloseCircle { get; } = EPackIcons.CloseCircle;

    public CustomEntryControl()
    {
        InitializeComponent();
    }

    private void InputView_OnTextChanged(object? sender, TextChangedEventArgs e)
        => IsPlaceholderVisible = string.IsNullOrEmpty(e.NewTextValue);

    private void TapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => Text = string.Empty;
}