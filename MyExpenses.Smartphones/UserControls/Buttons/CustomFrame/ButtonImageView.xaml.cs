using MyExpenses.Smartphones.PackIcons;

namespace MyExpenses.Smartphones.UserControls.Buttons.CustomFrame;

public sealed partial class ButtonImageView
{
    public static readonly BindableProperty GeometryColorProperty =
        BindableProperty.Create(nameof(GeometryColor), typeof(Color), typeof(ButtonImageTextView));

    public Color GeometryColor
    {
        get => (Color)GetValue(GeometryColorProperty);
        set => SetValue(GeometryColorProperty, value);
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

    public ButtonImageView()
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