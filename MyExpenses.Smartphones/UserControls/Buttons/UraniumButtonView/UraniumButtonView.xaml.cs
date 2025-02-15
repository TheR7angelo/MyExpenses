using MyExpenses.Smartphones.PackIcons;

namespace MyExpenses.Smartphones.UserControls.Buttons.UraniumButtonView;

public partial class UraniumButtonView
{
    public static readonly BindableProperty PathFillColorProperty =
        BindableProperty.Create(nameof(PathFillColor), typeof(Color), typeof(UraniumButtonView));

    public Color PathFillColor
    {
        get => (Color)GetValue(PathFillColorProperty);
        set => SetValue(PathFillColorProperty, value);
    }

    public static readonly BindableProperty PathSourceProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(PathSource), typeof(EPackIcons), typeof(UraniumButtonView), default(EPackIcons));

    public EPackIcons PathSource
    {
        get => (EPackIcons)GetValue(PathSourceProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(PathSourceProperty, value);
    }

    public static readonly BindableProperty PathHeightRequestProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(PathHeightRequest), typeof(double), typeof(UraniumButtonView), 0d);

    public double PathHeightRequest
    {
        get => (double)GetValue(PathHeightRequestProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(PathHeightRequestProperty, value);
    }

    public static readonly BindableProperty PathWidthRequestProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(PathWidthRequest), typeof(double), typeof(UraniumButtonView), 0d);

    public double PathWidthRequest
    {
        get => (double)GetValue(PathWidthRequestProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(PathWidthRequestProperty, value);
    }

    public static readonly BindableProperty PathVerticalOptionsProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(PathVerticalOptions), typeof(LayoutOptions), typeof(UraniumButtonView),
            default(LayoutOptions));

    public LayoutOptions PathVerticalOptions
    {
        get => (LayoutOptions)GetValue(PathVerticalOptionsProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(PathVerticalOptionsProperty, value);
    }

    public static readonly BindableProperty PathHorizontalOptionsProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(PathHorizontalOptions), typeof(LayoutOptions), typeof(UraniumButtonView),
            default(LayoutOptions));

    public LayoutOptions PathHorizontalOptions
    {
        get => (LayoutOptions)GetValue(PathHorizontalOptionsProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(PathHorizontalOptionsProperty, value);
    }

    public UraniumButtonView()
    {
        InitializeComponent();
    }
}