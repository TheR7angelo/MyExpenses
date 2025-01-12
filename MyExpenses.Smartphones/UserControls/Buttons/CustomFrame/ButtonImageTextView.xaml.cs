using MyExpenses.Smartphones.PackIcons;

namespace MyExpenses.Smartphones.UserControls.Buttons.CustomFrame;

public sealed partial class ButtonImageTextView
{
    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ButtonImageTextView));

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty GeometrySourceProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(GeometrySource), typeof(EPackIcons), typeof(ButtonImageTextView), default(EPackIcons));

    public EPackIcons GeometrySource
    {
        get => (EPackIcons)GetValue(GeometrySourceProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(GeometrySourceProperty, value);
    }

    public static readonly BindableProperty VerticalLabelOptionsProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(VerticalLabelOptions), typeof(LayoutOptions), typeof(ButtonImageTextView),
            default(LayoutOptions));

    public LayoutOptions VerticalLabelOptions
    {
        get => (LayoutOptions)GetValue(VerticalLabelOptionsProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(VerticalLabelOptionsProperty, value);
    }

    public static readonly BindableProperty HorizontalLabelOptionsProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(HorizontalLabelOptions), typeof(LayoutOptions), typeof(ButtonImageTextView),
            default(LayoutOptions));

    public LayoutOptions HorizontalLabelOptions
    {
        get => (LayoutOptions)GetValue(HorizontalLabelOptionsProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(HorizontalLabelOptionsProperty, value);
    }

    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ButtonImageTextView));

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly BindableProperty ImageHeightRequestProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(ImageHeightRequest), typeof(double), typeof(ButtonImageTextView), 0d);

    public double ImageHeightRequest
    {
        get => (double)GetValue(ImageHeightRequestProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(ImageHeightRequestProperty, value);
    }

    public static readonly BindableProperty ImageWidthRequestProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(ImageWidthRequest), typeof(double), typeof(ButtonImageTextView), 0d);

    public double ImageWidthRequest
    {
        get => (double)GetValue(ImageWidthRequestProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(ImageWidthRequestProperty, value);
    }

    public static readonly BindableProperty VerticalImageOptionsProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(VerticalImageOptions), typeof(LayoutOptions), typeof(ButtonImageTextView),
            default(LayoutOptions));

    public LayoutOptions VerticalImageOptions
    {
        get => (LayoutOptions)GetValue(VerticalImageOptionsProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(VerticalImageOptionsProperty, value);
    }

    public static readonly BindableProperty HorizontalImageOptionsProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(HorizontalImageOptions), typeof(LayoutOptions), typeof(ButtonImageTextView),
            default(LayoutOptions));

    public LayoutOptions HorizontalImageOptions
    {
        get => (LayoutOptions)GetValue(HorizontalImageOptionsProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(HorizontalImageOptionsProperty, value);
    }

    public event EventHandler? Clicked;

    public ButtonImageTextView()
    {
        InitializeComponent();
    }

    private void OnClicked()
    {
        Clicked?.Invoke(this, EventArgs.Empty);
    }

    private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
        => OnClicked();
}