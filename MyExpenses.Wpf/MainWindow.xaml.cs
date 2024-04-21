using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf;

public partial class MainWindow
{
    public ObservableCollection<VHistory> VHistories { get; }

    public MainWindow()
    {
        using var context = new DataBaseContext();
        VHistories = new ObservableCollection<VHistory>(context.VHistories.OrderByDescending(s => s.Date));

        InitializeComponent();
    }
}