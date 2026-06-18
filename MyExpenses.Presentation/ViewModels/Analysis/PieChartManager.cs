using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;

namespace MyExpenses.Presentation.ViewModels.Analysis;

public class PieChartManager(IPieChartView pieChart, ObservableCollection<CategoryTotalViewModel> categoryTotals)
{
    /// <summary>
    /// Updates the UI of the pie chart and the associated category totals based on the provided data.
    /// </summary>
    /// <param name="categoriesTotals">A collection of category total data, each containing category details, total values, symbols, and color codes.</param>
    /// <param name="grandTotal">The grand total value used to calculate percentages for the pie chart and category totals.</param>
    public void UpdateChartUi(IEnumerable<CategoryTotalDataViewModel> categoriesTotals, double grandTotal)
    {
        var series = (ObservableCollection<ISeries>)pieChart.Series;
        series.UpdateChartUi(categoryTotals, categoriesTotals, grandTotal);;
    }
}