using Serilog;
using UraniumUI.Material.Controls;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class ColorPickerPopup
{
    public static readonly BindableProperty RedValueProperty = BindableProperty.Create(nameof(RedValue), typeof(int),
        typeof(ColorPickerPopup), 0);

    public int? RedValue
    {
        get => (int?)GetValue(RedValueProperty);
        set => SetValue(RedValueProperty, value);
    }

    public static readonly BindableProperty GreenValueProperty = BindableProperty.Create(nameof(GreenValue), typeof(int),
        typeof(ColorPickerPopup), 0);

    public int? GreenValue
    {
        get => (int?)GetValue(GreenValueProperty);
        set => SetValue(GreenValueProperty, value);
    }

    public static readonly BindableProperty BlueValueProperty = BindableProperty.Create(nameof(BlueValue), typeof(int),
        typeof(ColorPickerPopup), 0);

    public int? BlueValue
    {
        get => (int?)GetValue(BlueValueProperty);
        set => SetValue(BlueValueProperty, value);
    }

    public ColorPickerPopup()
    {
        InitializeComponent();
    }

    private void TextField_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue.Equals(e.OldTextValue, StringComparison.InvariantCultureIgnoreCase)) return;
        if (sender is not TextField textField) return;

        Log.Information("New text value: {NewTextValue}", textField.Text);

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
}