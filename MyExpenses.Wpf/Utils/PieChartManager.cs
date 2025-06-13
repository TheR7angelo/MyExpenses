using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.WPF;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.Share.Core.Analysis;

namespace MyExpenses.Wpf.Utils;

public class PieChartManager(PieChart pieChart, ObservableCollection<CategoryTotal> categoryTotals)
{
    private PieChart PieChart { get; } = pieChart;

    private ObservableCollection<CategoryTotal> CategoryTotals { get; } = categoryTotals;

    /// <summary>
    /// Updates the UI of the pie chart and the associated category totals based on the provided data.
    /// </summary>
    /// <param name="categoriesTotals">A collection of category total data, each containing category details, total values, symbols, and color codes.</param>
    /// <param name="grandTotal">The grand total value used to calculate percentages for the pie chart and category totals.</param>
    public void UpdateChartUi(IEnumerable<CategoryTotalData> categoriesTotals, double grandTotal)
    {
        var series = (ObservableCollection<ISeries>)PieChart.Series;
        series.UpdateChartUi(CategoryTotals, categoriesTotals, grandTotal);;
    }
}