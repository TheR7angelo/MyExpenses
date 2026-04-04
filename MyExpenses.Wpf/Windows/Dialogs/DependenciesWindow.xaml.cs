using System.Windows;

namespace MyExpenses.Wpf.Windows.Dialogs;

public partial class DependenciesWindow
{
    public static readonly DependencyProperty DeletingNameProperty = DependencyProperty.Register(nameof(DeletingName),
        typeof(string), typeof(DependenciesWindow), new PropertyMetadata(default(string)));

    public string DeletingName
    {
        get => (string)GetValue(DeletingNameProperty);
        set => SetValue(DeletingNameProperty, value);
    }

    public DependenciesWindow()
    {
        InitializeComponent();
    }
}