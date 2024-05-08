﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditColorWindow : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private Color _color = Colors.Black;

    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            UpdateSlider();
        }
    }

    public byte RedValue
    {
        get => _color.R;
        set
        {
            _color = Color.FromArgb(_color.A, value, _color.G, _color.B);
            OnPropertyChanged();
            OnPropertyChanged(nameof(Color));
        }
    }

    public byte GreenValue
    {
        get => _color.G;
        set
        {
            _color = Color.FromArgb(_color.A, _color.R, value, _color.B);
            OnPropertyChanged();
            OnPropertyChanged(nameof(Color));
        }
    }

    public byte BlueValue
    {
        get => _color.B;
        set
        {
            _color = Color.FromArgb(_color.A, _color.R, _color.G, value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(Color));
        }
    }

    public byte AlphaValue
    {
        get => _color.A;
        set
        {
            _color = Color.FromArgb(value, _color.R, _color.G, _color.B);
            OnPropertyChanged();
            OnPropertyChanged(nameof(Color));
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