using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.IO;
using MyExpenses.Wpf.Resources.Resx.Windows.RemoveDatabaseFile;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class RemoveDatabaseFile
{
    public string ButtonCancelContent { get; } = RemoveDatabaseFileResources.ButtonCancelCotent;
    public string ButtonValidContent { get; } = RemoveDatabaseFileResources.ButtonValidContent;

    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];

    internal List<ExistingDatabase> ExistingDatabasesToDelete { get; } = [];

    public RemoveDatabaseFile()
    {
        InitializeComponent();
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var checkBoxesChecked = ListView.FindVisualChildren<CheckBox>()
            .Where(s => (bool)s.IsChecked!).ToList();

        foreach (var existingDatabase in checkBoxesChecked.Select(checkBoxChecked => checkBoxChecked.DataContext as ExistingDatabase).OfType<ExistingDatabase>())
        {
            ExistingDatabasesToDelete.Add(existingDatabase);
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