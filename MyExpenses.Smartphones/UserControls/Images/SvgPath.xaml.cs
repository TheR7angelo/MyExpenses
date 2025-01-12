using MyExpenses.Smartphones.PackIcons;

namespace MyExpenses.Smartphones.UserControls.Images;

public partial class SvgPath
{
    public static readonly BindableProperty GeometrySourceProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(GeometrySource), typeof(EPackIcons), typeof(SvgPath), default(EPackIcons));

    public EPackIcons GeometrySource
    {
        get => (EPackIcons)GetValue(GeometrySourceProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(GeometrySourceProperty, value);
    }

    public static readonly BindableProperty GeometryColorProperty =
        BindableProperty.Create(nameof(GeometryColor), typeof(Color), typeof(SvgPath));

    public Color GeometryColor
    {
        get => (Color)GetValue(GeometryColorProperty);
        set => SetValue(GeometryColorProperty, value);
    }

    public event EventHandler? Clicked;

    public SvgPath()
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