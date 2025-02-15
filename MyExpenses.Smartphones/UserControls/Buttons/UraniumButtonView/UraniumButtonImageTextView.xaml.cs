using MyExpenses.Smartphones.PackIcons;

namespace MyExpenses.Smartphones.UserControls.Buttons.UraniumButtonView;

public partial class UraniumButtonImageTextView
{
    public static readonly BindableProperty LabelVerticalOptionsProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(LabelVerticalOptions), typeof(LayoutOptions), typeof(UraniumButtonImageTextView),
            default(LayoutOptions));

    public LayoutOptions LabelVerticalOptions
    {
        get => (LayoutOptions)GetValue(LabelVerticalOptionsProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(LabelVerticalOptionsProperty, value);
    }

    public static readonly BindableProperty LabelHorizontalOptionsProperty =
        BindableProperty.Create(nameof(LabelHorizontalOptions), typeof(LayoutOptions),
            // ReSharper disable once HeapView.BoxingAllocation
            typeof(UraniumButtonImageTextView), default(LayoutOptions));

    public LayoutOptions LabelHorizontalOptions
    {
        get => (LayoutOptions)GetValue(LabelHorizontalOptionsProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(LabelHorizontalOptionsProperty, value);
    }

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(UraniumButtonImageTextView));

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty GeometrySourceProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(GeometrySource), typeof(EPackIcons), typeof(UraniumButtonImageTextView),
            default(EPackIcons));

    public EPackIcons GeometrySource
    {
        get => (EPackIcons)GetValue(GeometrySourceProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(GeometrySourceProperty, value);
    }

    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(UraniumButtonImageTextView));

    public string Text
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly BindableProperty ImageHeightRequestProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(ImageHeightRequest), typeof(double), typeof(UraniumButtonImageTextView), 0d);

    public double ImageHeightRequest
    {
        get => (double)GetValue(ImageHeightRequestProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(ImageHeightRequestProperty, value);
    }

    public static readonly BindableProperty ImageWidthRequestProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(ImageWidthRequest), typeof(double), typeof(UraniumButtonImageTextView), 0d);

    public double ImageWidthRequest
    {
        get => (double)GetValue(ImageWidthRequestProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(ImageWidthRequestProperty, value);
    }

    public static readonly BindableProperty VerticalImageOptionsProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(VerticalImageOptions), typeof(LayoutOptions), typeof(UraniumButtonImageTextView),
            default(LayoutOptions));

    public LayoutOptions VerticalImageOptions
    {
        get => (LayoutOptions)GetValue(VerticalImageOptionsProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(VerticalImageOptionsProperty, value);
    }

    public static readonly BindableProperty HorizontalImageOptionsProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(HorizontalImageOptions), typeof(LayoutOptions),
            typeof(UraniumButtonImageTextView),
            default(LayoutOptions));

    public LayoutOptions HorizontalImageOptions
    {
        get => (LayoutOptions)GetValue(HorizontalImageOptionsProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(HorizontalImageOptionsProperty, value);
    }

    public UraniumButtonImageTextView()
    {
        InitializeComponent();
    }
}