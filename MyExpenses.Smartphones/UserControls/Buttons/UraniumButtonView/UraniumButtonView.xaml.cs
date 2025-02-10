using MyExpenses.Smartphones.PackIcons;

namespace MyExpenses.Smartphones.UserControls.Buttons.UraniumButtonView;

public partial class UraniumButtonView
{
    public static readonly BindableProperty GeometryColorProperty =
        BindableProperty.Create(nameof(GeometryColor), typeof(Color), typeof(UraniumButtonView));

    public Color GeometryColor
    {
        get => (Color)GetValue(GeometryColorProperty);
        set => SetValue(GeometryColorProperty, value);
    }

    public static readonly BindableProperty GeometrySourceProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(GeometrySource), typeof(EPackIcons), typeof(UraniumButtonView), default(EPackIcons));

    public EPackIcons GeometrySource
    {
        get => (EPackIcons)GetValue(GeometrySourceProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(GeometrySourceProperty, value);
    }

    public static readonly BindableProperty ImageHeightRequestProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(ImageHeightRequest), typeof(double), typeof(UraniumButtonView), 0d);

    public double ImageHeightRequest
    {
        get => (double)GetValue(ImageHeightRequestProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(ImageHeightRequestProperty, value);
    }

    public static readonly BindableProperty ImageWidthRequestProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(ImageWidthRequest), typeof(double), typeof(UraniumButtonView), 0d);

    public double ImageWidthRequest
    {
        get => (double)GetValue(ImageWidthRequestProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(ImageWidthRequestProperty, value);
    }

    public static readonly BindableProperty VerticalImageOptionsProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(VerticalImageOptions), typeof(LayoutOptions), typeof(UraniumButtonView),
            default(LayoutOptions));

    public LayoutOptions VerticalImageOptions
    {
        get => (LayoutOptions)GetValue(VerticalImageOptionsProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(VerticalImageOptionsProperty, value);
    }

    public static readonly BindableProperty HorizontalImageOptionsProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(HorizontalImageOptions), typeof(LayoutOptions), typeof(UraniumButtonView),
            default(LayoutOptions));

    public LayoutOptions HorizontalImageOptions
    {
        get => (LayoutOptions)GetValue(HorizontalImageOptionsProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(HorizontalImageOptionsProperty, value);
    }

    public UraniumButtonView()
    {
        InitializeComponent();
    }
}