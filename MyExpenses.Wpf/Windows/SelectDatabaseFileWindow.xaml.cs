using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using MyExpenses.Models.IO;
using MyExpenses.Utils.WindowStyle;
using MyExpenses.Wpf.Resources.Resx.Windows.RemoveDatabaseFile;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class SelectDatabaseFileWindow
{
    public string ButtonCancelContent { get; } = SelectDatabaseFileWindowResources.ButtonCancelCotent;
    public string ButtonValidContent { get; } = SelectDatabaseFileWindowResources.ButtonValidContent;

    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];

    internal List<ExistingDatabase> ExistingDatabasesSelected { get; } = [];

    public SelectDatabaseFileWindow()
    {
        InitializeComponent();

        var hWnd = new WindowInteropHelper(GetWindow(this)!).EnsureHandle();
        hWnd.SetWindowCornerPreference(DwmWindowCornerPreference.Round);
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var checkBoxesChecked = ListView.FindVisualChildren<CheckBox>()
            .Where(s => (bool)s.IsChecked!).ToList();

        foreach (var existingDatabase in checkBoxesChecked.Select(checkBoxChecked => checkBoxChecked.DataContext as ExistingDatabase).OfType<ExistingDatabase>())
        {
            ExistingDatabasesSelected.Add(existingDatabase);
        }

        DialogResult = true;
        Close();
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}