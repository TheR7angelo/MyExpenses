using System.Collections;
using MyExpenses.Smartphones.PackIcons;

namespace MyExpenses.Smartphones.UserControls.Pickers;

public partial class CustomPicker
{
    public static readonly BindableProperty GeometryColorProperty =
        BindableProperty.Create(nameof(GeometryColor), typeof(Color), typeof(CustomPicker), default(Color));

    public Color GeometryColor
    {
        get => (Color)GetValue(GeometryColorProperty);
        set => SetValue(GeometryColorProperty, value);
    }

    public static readonly BindableProperty HasClearButtonProperty =
        BindableProperty.Create(nameof(HasClearButton), typeof(bool), typeof(CustomPicker), default(bool));

    public bool HasClearButton
    {
        get => (bool)GetValue(HasClearButtonProperty);
        set => SetValue(HasClearButtonProperty, value);
    }

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(
        object), typeof(CustomPicker), default(object));

    public object? SelectedItem
    {
        get => (object?)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(CustomPicker), default(IList));

    public IList ItemsSource
    {
        get => (IList)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty IsPlaceholderVisibleProperty =
        BindableProperty.Create(nameof(IsPlaceholderVisible), typeof(bool), typeof(CustomPicker), default(bool));

    public bool IsPlaceholderVisible
    {
        get => (bool)GetValue(IsPlaceholderVisibleProperty);
        set => SetValue(IsPlaceholderVisibleProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty =
        BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(CustomPicker), default(string));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public EPackIcons CloseCircle { get; } = EPackIcons.CloseCircle;

    public event EventHandler? SelectedIndexChanged;

    public CustomPicker()
    {
        InitializeComponent();
    }

    private void Picker_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        SelectedIndexChanged?.Invoke(sender, e);

        IsPlaceholderVisible = SelectedItem is null;
    }

    private void TapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => SelectedItem = null;
}