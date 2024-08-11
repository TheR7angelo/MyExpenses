using System.Windows;
using MyExpenses.Models.Sql.Views;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class StackedTotalEllipseControl
{
    public static readonly DependencyProperty VTotalByAccountProperty =
        DependencyProperty.Register(nameof(VTotalByAccount), typeof(VTotalByAccount),
            typeof(StackedTotalEllipseControl), new PropertyMetadata(default(VTotalByAccount)));

    public VTotalByAccount VTotalByAccount
    {
        get => (VTotalByAccount)GetValue(VTotalByAccountProperty);
        set => SetValue(VTotalByAccountProperty, value);
    }

    public StackedTotalEllipseControl()
    {
        InitializeComponent();
    }
}