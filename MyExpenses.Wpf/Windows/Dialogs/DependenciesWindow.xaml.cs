using System.Collections.ObjectModel;
using System.Windows;
using Domain.Models.Dependencies;
using MyExpenses.SharedUtils.Collection;

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

    public ObservableCollection<DeletionDependency> Dependencies { get; } = [];

    public DependenciesWindow()
    {
        InitializeComponent();
    }

    public void SetDependencies(IEnumerable<DeletionDependency> deletionDependencies)
    {
        Dependencies.Clear();
        Dependencies.AddRange(deletionDependencies);
    }

    private void ButtonConfirmation_OnClick(object sender, RoutedEventArgs e)
        => SetResult(true);

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        => SetResult(false);

    private void SetResult(bool result)
    {
        DialogResult = result;
        Close();
    }
}