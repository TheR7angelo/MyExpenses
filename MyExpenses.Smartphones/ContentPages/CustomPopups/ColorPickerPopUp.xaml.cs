using MyExpenses.Models.Config.Interfaces;
using UraniumUI.Material.Controls;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class ColorPickerPopup
{
    public static readonly BindableProperty AlphaValueProperty = BindableProperty.Create(nameof(AlphaValue), typeof(int),
            typeof(ColorPickerPopup), 255, propertyChanged: ColorValue_PropertyChanged);

    public int? AlphaValue
    {
        get => (int?)GetValue(AlphaValueProperty);
        set => SetValue(AlphaValueProperty, value);
    }

    public static readonly BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(ColorPickerPopup), Colors.Black);

    public Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    public static readonly BindableProperty RedValueProperty = BindableProperty.Create(nameof(RedValue), typeof(int),
        typeof(ColorPickerPopup), propertyChanged: ColorValue_PropertyChanged);

    public int? RedValue
    {
        get => (int?)GetValue(RedValueProperty);
        set => SetValue(RedValueProperty, value);
    }

    public static readonly BindableProperty GreenValueProperty = BindableProperty.Create(nameof(GreenValue), typeof(int),
        typeof(ColorPickerPopup), propertyChanged: ColorValue_PropertyChanged);

    public int? GreenValue
    {
        get => (int?)GetValue(GreenValueProperty);
        set => SetValue(GreenValueProperty, value);
    }

    public static readonly BindableProperty BlueValueProperty = BindableProperty.Create(nameof(BlueValue), typeof(int),
        typeof(ColorPickerPopup), propertyChanged: ColorValue_PropertyChanged);

    public int? BlueValue
    {
        get => (int?)GetValue(BlueValueProperty);
        set => SetValue(BlueValueProperty, value);
    }

    public ColorPickerPopup()
    {
        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UpdateLanguage()
    {

    }

    private void TextField_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue.Equals(e.OldTextValue, StringComparison.InvariantCultureIgnoreCase)) return;
        if (sender is not TextField textField) return;

        int? value = string.IsNullOrWhiteSpace(textField.Text)
            ? null
            : textField.Text.StartsWith('-')
                ? null
                : int.Parse(textField.Text);

        switch (value)
        {
            case > 255:
                value = 255;
                break;
            case < 0:
                value = 0;
                break;
        }

        textField.Text = value.ToString();
    }

    private static void ColorValue_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var sender = (ColorPickerPopup)bindable;
        sender.UpdateColor();
    }

    private void UpdateColor()
        => BackgroundColor = Color.FromRgba(RedValue ?? 0, GreenValue ?? 0, BlueValue ?? 0, AlphaValue ?? 0);
}