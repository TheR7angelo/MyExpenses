using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.Utils;

namespace MyExpenses.Share.Core.Analysis;

public static class PieChartManagerUtils
{
    /// <summary>
    /// Updates the UI of the Pie Chart by syncing the series and category totals with the updated data.
    /// </summary>
    /// <param name="series">The collection of pie series used to populate the chart.</param>
    /// <param name="categoryTotals">A collection representing the category totals being visualized.</param>
    /// <param name="categoriesTotals">The updated data for categories and their totals.</param>
    /// <param name="grandTotal">The aggregate total value across all categories.</param>
    public static void UpdateChartUi(this ObservableCollection<ISeries> series,
        ObservableCollection<CategoryTotal> categoryTotals,
        IEnumerable<CategoryTotalData> categoriesTotals, double grandTotal)
    {
        var existingSeries = series
            .OfType<PieSeries<double>>()
            .ToDictionary(s => (string)s.Tag!, s => s);

        var categoryTotalsDict = categoryTotals.ToDictionary(ct => ct.Name!, ct => ct);

        var categoryTotalDatas = categoriesTotals as CategoryTotalData[] ?? categoriesTotals.ToArray();

        foreach (var categoryTotal in categoryTotalDatas)
        {
            var absTotal = Math.Abs(categoryTotal.Total);
            var percentage = grandTotal != 0 ? Math.Round(absTotal / grandTotal * 100, 2) : 0;

            series.UpdateOrCreatePieSeries(existingSeries, categoryTotal, absTotal, percentage);
            categoryTotals.UpdateOrCreateCategoryTotal(categoryTotalsDict, categoryTotal, percentage);
        }

        series.RemoveObsoleteElements(existingSeries, categoryTotals, categoryTotalsDict, categoryTotalDatas);
    }

    private static void UpdateOrCreatePieSeries(this ObservableCollection<ISeries> series, Dictionary<string, PieSeries<double>> existingSeries,
        CategoryTotalData categoryTotal, double absTotal, double percentage)
    {
        var multiplier = categoryTotal.Total < 0 ? -1 : 1;

        var func = categoryTotal.Symbol!.CreateDoughnutLabelFunc(multiplier);
        var solidColorPaint = categoryTotal.HexadecimalColorCode.ToSolidColorPaint();

        if (existingSeries.TryGetValue(categoryTotal.Category!, out var pieSeries))
        {
            pieSeries.Values = [absTotal];
            pieSeries.Name = $"{categoryTotal.Category} ({percentage}%)";
            pieSeries.ToolTipLabelFormatter = func;

            if (pieSeries.Fill is not SolidColorPaint solidColorPaintPaint || solidColorPaintPaint.Color != solidColorPaint?.Color)
            {
                pieSeries.Fill = solidColorPaint;
            }
        }
        else
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The PieSeries instance is created with the specified values to handle pie series operations.
            pieSeries = new PieSeries<double>
            {
                Values = [absTotal],
                Name = $"{categoryTotal.Category} ({percentage}%)",
                ToolTipLabelFormatter = func,
                Fill = solidColorPaint,
                Tag = categoryTotal.Category
            };

            series.Add(pieSeries);
        }
    }

    private static void UpdateOrCreateCategoryTotal(this ObservableCollection<CategoryTotal> categoryTotals, Dictionary<string, CategoryTotal> categoryTotalsDict,
        CategoryTotalData categoryTotal, double percentage)
    {
        if (categoryTotalsDict.TryGetValue(categoryTotal.Category!, out var existingCategoryTotal))
        {
            existingCategoryTotal.HexadecimalColor = categoryTotal.HexadecimalColorCode;
            existingCategoryTotal.Percentage = percentage;
            existingCategoryTotal.Value = categoryTotal.Total;
            existingCategoryTotal.Symbol = categoryTotal.Symbol;
        }
        else
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The CategoryTotal instance is created with the specified values to handle category total operations.
            var newCategoryTotal = new CategoryTotal
            {
                Name = categoryTotal.Category,
                HexadecimalColor = categoryTotal.HexadecimalColorCode,
                Percentage = percentage,
                Value = categoryTotal.Total,
                Symbol = categoryTotal.Symbol
            };

            categoryTotals.Add(newCategoryTotal);
            categoryTotalsDict[categoryTotal.Category!] = newCategoryTotal;
        }
    }

    private static void RemoveObsoleteElements(this ObservableCollection<ISeries> series,
        Dictionary<string, PieSeries<double>> existingSeries,
        ObservableCollection<CategoryTotal> categoryTotals,
        Dictionary<string, CategoryTotal> categoryTotalsDict,
        CategoryTotalData[] categoryTotalDatas)
    {
        var updatedCategories = categoryTotalDatas.Select(ct => ct.Category!).ToHashSet();

        foreach (var keyToRemove in existingSeries.Keys.Except(updatedCategories))
        {
            series.Remove(existingSeries[keyToRemove]);
        }

        foreach (var category in categoryTotalsDict.Keys.Except(updatedCategories))
        {
            categoryTotals.Remove(categoryTotalsDict[category]);
        }
    }
}