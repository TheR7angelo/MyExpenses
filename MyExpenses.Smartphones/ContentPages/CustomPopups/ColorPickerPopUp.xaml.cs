using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Objects;
using MyExpenses.SharedUtils.Resources.Resx.ColorManagement;
using UraniumUI.Material.Controls;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class ColorPickerPopup
{
    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(ColorPickerPopup));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty ButtonDeleteTextProperty = BindableProperty.Create(nameof(ButtonDeleteText),
        typeof(string), typeof(ColorPickerPopup));

    public string ButtonDeleteText
    {
        get => (string)GetValue(ButtonDeleteTextProperty);
        set => SetValue(ButtonDeleteTextProperty, value);
    }

    public static readonly BindableProperty ButtonCancelTextProperty = BindableProperty.Create(nameof(ButtonCancelText),
        typeof(string), typeof(ColorPickerPopup));

    public string ButtonCancelText
    {
        get => (string)GetValue(ButtonCancelTextProperty);
        set => SetValue(ButtonCancelTextProperty, value);
    }

    public static readonly BindableProperty EditColorProperty =
        BindableProperty.Create(nameof(EditColor), typeof(bool), typeof(ColorPickerPopup), false);

    public bool EditColor
    {
        get => (bool)GetValue(EditColorProperty);
        set => SetValue(EditColorProperty, value);
    }

    public static readonly BindableProperty TextBoxColorNameProperty = BindableProperty.Create(nameof(TextBoxColorName),
        typeof(string), typeof(ColorPickerPopup));

    public string TextBoxColorName
    {
        get => (string)GetValue(TextBoxColorNameProperty);
        set => SetValue(TextBoxColorNameProperty, value);
    }

    public static readonly BindableProperty ColorNameProperty =
        BindableProperty.Create(nameof(ColorName), typeof(string), typeof(ColorPickerPopup));

    public string? ColorName
    {
        get => (string?)GetValue(ColorNameProperty);
        set => SetValue(ColorNameProperty, value);
    }

    public static readonly BindableProperty LabelAlphaChannelProperty =
        BindableProperty.Create(nameof(LabelAlphaChannel), typeof(string), typeof(ColorPickerPopup));

    public string LabelAlphaChannel
    {
        get => (string)GetValue(LabelAlphaChannelProperty);
        set => SetValue(LabelAlphaChannelProperty, value);
    }

    public static readonly BindableProperty LabelRedChannelProperty = BindableProperty.Create(nameof(LabelRedChannel),
        typeof(string), typeof(ColorPickerPopup));

    public string LabelRedChannel
    {
        get => (string)GetValue(LabelRedChannelProperty);
        set => SetValue(LabelRedChannelProperty, value);
    }

    public static readonly BindableProperty LabelGreenChannelProperty =
        BindableProperty.Create(nameof(LabelGreenChannel), typeof(string), typeof(ColorPickerPopup));

    public string LabelGreenChannel
    {
        get => (string)GetValue(LabelGreenChannelProperty);
        set => SetValue(LabelGreenChannelProperty, value);
    }

    public static readonly BindableProperty LabelBlueChannelProperty = BindableProperty.Create(nameof(LabelBlueChannel),
        typeof(string), typeof(ColorPickerPopup));

    public string LabelBlueChannel
    {
        get => (string)GetValue(LabelBlueChannelProperty);
        set => SetValue(LabelBlueChannelProperty, value);
    }

    public static readonly BindableProperty LabelHexadecimalCodeProperty =
        BindableProperty.Create(nameof(LabelHexadecimalCode), typeof(string), typeof(ColorPickerPopup));

    public string LabelHexadecimalCode
    {
        get => (string)GetValue(LabelHexadecimalCodeProperty);
        set => SetValue(LabelHexadecimalCodeProperty, value);
    }

    public static readonly BindableProperty AlphaValueProperty = BindableProperty.Create(nameof(AlphaValue), typeof(int),
            typeof(ColorPickerPopup), 255, propertyChanged: ColorValue_PropertyChanged);

    public int? AlphaValue
    {
        get => (int?)GetValue(AlphaValueProperty);
        set => SetValue(AlphaValueProperty, value);
    }

    public static readonly BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(ColorPickerPopup), Colors.Black, propertyChanged: BackgroundColor_PropertyChanged);

    private static void BackgroundColor_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var sender = (ColorPickerPopup)bindable;
        sender.UpdateRgba();
    }

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

    private TColor? TColor { get; set; }

    public int MaxLength { get; }

    public ColorPickerPopup()
    {
        MaxLength = Utils.Converters.MaxLengthConverter.Convert(typeof(TColor), nameof(TColor.Name));

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        LabelRedChannel = ColorManagementResources.LabelRedChannel;
        LabelGreenChannel = ColorManagementResources.LabelGreenChannel;
        LabelBlueChannel = ColorManagementResources.LabelBlueChannel;
        LabelAlphaChannel = ColorManagementResources.LabelAlphaChannel;
        LabelHexadecimalCode = ColorManagementResources.LabelHexadecimalCode;
        TextBoxColorName = ColorManagementResources.TextBoxColorName;

        ButtonValidText = ColorManagementResources.ButtonValidContent;
        ButtonDeleteText = ColorManagementResources.ButtonDeleteContent;
        ButtonCancelText = ColorManagementResources.ButtonCancelContent;
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

    private bool _isUpdateSlider;

    private void UpdateColor()
    {
        if (!_isUpdateSlider) BackgroundColor = Color.FromRgba(RedValue ?? 0, GreenValue ?? 0, BlueValue ?? 0, AlphaValue ?? 0);
    }

    private void UpdateRgba()
    {
        _isUpdateSlider = true;
        BackgroundColor.ToRgba(out var r, out var g, out var b, out var a);
        RedValue = r;
        GreenValue = g;
        BlueValue = b;
        AlphaValue = a;
        _isUpdateSlider = false;
    }

    private void TextFieldColorHexadecimal_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextField textField) return;
        if (string.IsNullOrWhiteSpace(textField.Text)) return;

        var txt = textField.Text.ToUpper();
        if (textField.Text.Length > 9) txt = txt[..9];

        var correctedText = new char[txt.Length];
        for (var i = 0; i < txt.Length; i++)
        {
            var currentChar = txt[i];
            if (char.IsLetter(currentChar) && currentChar > 'F') correctedText[i] = 'F';
            else correctedText[i] = currentChar;
        }
        textField.Text = new string(correctedText);
    }

    /// <summary>
    /// Sets the color of the ColorPickerPopup based on the provided TColor object.
    /// </summary>
    /// <param name="tColor">The TColor object containing the color value to be applied.</param>
    public void SetColor(TColor tColor)
    {
        var color = Color.FromArgb(tColor.HexadecimalColorCode);
        BackgroundColor = color;
        ColorName = tColor.Name;

        TColor = tColor.DeepCopy();
    }

    private void ButtonValid_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ButtonDelete_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ButtonCancel_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}