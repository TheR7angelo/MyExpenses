using System.Collections.ObjectModel;
using MyExpenses.Models.IO;
using MyExpenses.Utils.Collection;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class VacuumDatabaseUpdateWindow
{
    public ObservableCollection<SizeDatabase> SizeDatabases { get; } = [];

    public VacuumDatabaseUpdateWindow(SizeDatabase sizeDatabase)
    {
        SizeDatabases.Add(sizeDatabase);
        InitializeComponent();

        SetRoundWindow();
    }

    public VacuumDatabaseUpdateWindow(IEnumerable<SizeDatabase> sizeDatabases)
    {
        SizeDatabases.AddRangeAndSort(sizeDatabases, s => s.FileNameWithoutExtension);
        InitializeComponent();

        SetRoundWindow();
    }

    private void SetRoundWindow()
        => this.SetWindowCornerPreference();

}