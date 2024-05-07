using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditColorWindow : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private Color _color = Colors.Transparent;

    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            UpdateSlider();
        }
    }

    private void UpdateSlider()
    {
        var redGradientStart = Color.FromArgb(Color.A, 0, Color.G, Color.B);
        var redGradientStop = Color.FromArgb(Color.A, 255, Color.G, Color.B);
        RedGradientStart.Color = redGradientStart;
        RedGradientStop.Color = redGradientStop;

        var greenGradientStart = Color.FromArgb(Color.A, Color.R, 0, Color.B);
        var greenGradientStop = Color.FromArgb(Color.A, Color.R, 255, Color.B);
        GreenGradientStart.Color = greenGradientStart;
        GreenGradientStop.Color = greenGradientStop;

        var blueGradientStart = Color.FromArgb(Color.A, Color.R, Color.G, 0);
        var blueGradientStop = Color.FromArgb(Color.A, Color.R, Color.G, 255);
        BlueGradientStart.Color = blueGradientStart;
        BlueGradientStop.Color = blueGradientStop;

        var alphaGradientStart = Color.FromArgb(0, Color.R, Color.G, Color.B);
        var alphaGradientStop = Color.FromArgb(255, Color.R, Color.G, Color.B);
        AlphaGradientStart.Color = alphaGradientStart;
        AlphaGradientStop.Color = alphaGradientStop;
    }

    public AddEditColorWindow()
    {
        InitializeComponent();
    }
}