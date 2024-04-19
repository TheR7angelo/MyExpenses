using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf;

public partial class MainWindow
{
    public ObservableCollection<THistory> DayHistories { get; }

    public MainWindow()
    {
        using var context = new DataBaseContext();
        DayHistories = new ObservableCollection<THistory>(context.THistories);

        InitializeComponent();
    }
}