using System.Collections.ObjectModel;
using System.Windows.Interop;
using MyExpenses.Models.IO;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.WindowStyle;

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
    {
        var hWnd = new WindowInteropHelper(GetWindow(this)!).EnsureHandle();
        hWnd.SetWindowCornerPreference(DwmWindowCornerPreference.Round);
    }
}