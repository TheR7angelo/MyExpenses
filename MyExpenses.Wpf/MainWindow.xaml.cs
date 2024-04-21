using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf;

public partial class MainWindow
{
    public List<VHistory> VHistories { get; }

    public MainWindow()
    {
        using var context = new DataBaseContext();
        VHistories = new List<VHistory>(context.VHistories.OrderByDescending(s => s.Date));

        InitializeComponent();
    }
}