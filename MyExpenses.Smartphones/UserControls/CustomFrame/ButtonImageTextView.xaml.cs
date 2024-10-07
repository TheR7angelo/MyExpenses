using Microsoft.Maui.Controls.Shapes;

namespace MyExpenses.Smartphones.UserControls.CustomFrame;

public sealed partial class ButtonImageTextView
{
    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ButtonImageTextView), default(Color));

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty GeometrySourceProperty = BindableProperty.Create(nameof(GeometrySource),
        typeof(Geometry), typeof(ButtonImageTextView), default(Geometry));

    public Geometry GeometrySource
    {
        get => (Geometry)GetValue(GeometrySourceProperty);
        set => SetValue(GeometrySourceProperty, value);
    }

    public static readonly BindableProperty VerticalLabelOptionsProperty =
        BindableProperty.Create(nameof(VerticalLabelOptions), typeof(LayoutOptions), typeof(ButtonImageTextView),
            default(LayoutOptions));

    public LayoutOptions VerticalLabelOptions
    {
        get => (LayoutOptions)GetValue(VerticalLabelOptionsProperty);
        set => SetValue(VerticalLabelOptionsProperty, value);
    }

    public static readonly BindableProperty HorizontalLabelOptionsProperty =
        BindableProperty.Create(nameof(HorizontalLabelOptions), typeof(LayoutOptions), typeof(ButtonImageTextView),
            default(LayoutOptions));

    public LayoutOptions HorizontalLabelOptions
    {
        get => (LayoutOptions)GetValue(HorizontalLabelOptionsProperty);
        set => SetValue(HorizontalLabelOptionsProperty, value);
    }

    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ButtonImageTextView), default(string));

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly BindableProperty ImageHeightRequestProperty =
        BindableProperty.Create(nameof(ImageHeightRequest), typeof(double), typeof(ButtonImageTextView), default(double));

    public double ImageHeightRequest
    {
        get => (double)GetValue(ImageHeightRequestProperty);
        set => SetValue(ImageHeightRequestProperty, value);
    }

    public static readonly BindableProperty ImageWidthRequestProperty =
        BindableProperty.Create(nameof(ImageWidthRequest), typeof(double), typeof(ButtonImageTextView), default(double));

    public double ImageWidthRequest
    {
        get => (double)GetValue(ImageWidthRequestProperty);
        set => SetValue(ImageWidthRequestProperty, value);
    }

    public static readonly BindableProperty VerticalImageOptionsProperty =
        BindableProperty.Create(nameof(VerticalImageOptions), typeof(LayoutOptions), typeof(ButtonImageTextView),
            default(LayoutOptions));

    public LayoutOptions VerticalImageOptions
    {
        get => (LayoutOptions)GetValue(VerticalImageOptionsProperty);
        set => SetValue(VerticalImageOptionsProperty, value);
    }

    public static readonly BindableProperty HorizontalImageOptionsProperty =
        BindableProperty.Create(nameof(HorizontalImageOptions), typeof(LayoutOptions), typeof(ButtonImageTextView),
            default(LayoutOptions));

    public LayoutOptions HorizontalImageOptions
    {
        get => (LayoutOptions)GetValue(HorizontalImageOptionsProperty);
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