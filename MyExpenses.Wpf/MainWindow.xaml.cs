using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf;

public partial class MainWindow
{
    public ObservableCollection<VHistoryByDay> DayHistories { get; }

    public MainWindow()
    {
        using var context = new DataBaseContext();
        DayHistories = new ObservableCollection<VHistoryByDay>(context.VHistoryByDays.OrderByDescending(s => s.Date));

        InitializeComponent();
    }
}