using System.Collections.ObjectModel;
using MyExpenses.Models.IO;
using MyExpenses.Utils.Collection;

namespace MyExpenses.Wpf.Windows;

public partial class VacuumDatabaseUpdateWindow
{
    public ObservableCollection<SizeDatabase> SizeDatabases { get; } = [];

    public VacuumDatabaseUpdateWindow(SizeDatabase sizeDatabase)
    {
        SizeDatabases.Add(sizeDatabase);
        InitializeComponent();
    }

    public VacuumDatabaseUpdateWindow(IEnumerable<SizeDatabase> sizeDatabases)
    {
        SizeDatabases.AddRangeAndSort(sizeDatabases, s => s.FileNameWithoutExtension);
        InitializeComponent();
    }
}