using MyExpenses.Presentation.ViewModel;
using MyExpenses.Utils;

namespace MyExpenses.Wpf.Pages;

public partial class DashBoardPage
{
    public DashBoardPage(DashBoardViewModel dashBoardViewModel)
    {
        InitializeComponent();

        UpdatePieChartLegendTextPaint();

        DataContext = dashBoardViewModel;
    }

    #region Function

    private void UpdatePieChartLegendTextPaint()
    {
        var wpfColor = MyExpenses.Wpf.Utils.Resources.GetMaterialDesignBodySkColor();
        PieChart.LegendTextPaint = wpfColor.ToSolidColorPaint();
    }

    #endregion
}