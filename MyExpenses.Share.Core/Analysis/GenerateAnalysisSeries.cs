using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Utils;

namespace MyExpenses.Share.Core.Analysis;

public static class GenerateAnalysisSeries
{
    /// <summary>
    /// Generates positive and negative column series for the given records using specified parameters.
    /// </summary>
    /// <param name="records">The list of grouped records containing monthly positive and negative sums.</param>
    /// <param name="hexadecimalCodePrimaryColor">The hexadecimal color code for the primary color of the positive series.</param>
    /// <param name="hexadecimalCodeSecondaryColor">The hexadecimal color code for the secondary color of the negative series.</param>
    /// <param name="positiveName">The name/label for the positive column series.</param>
    /// <param name="negativeName">The name/label for the negative column series.</param>
    /// <returns>A tuple containing the positive and negative column series.</returns>
    public static (ColumnSeries<double> PositiveSeries, ColumnSeries<double> NegativeSeries) GenerateSeries(
        this List<IGrouping<string?, AnalysisVAccountCategoryMonthlySumPositiveNegative>> records,
        string hexadecimalCodePrimaryColor, string hexadecimalCodeSecondaryColor, string positiveName,
        string negativeName)
    {
        var positiveValues = records.Select(g =>
            Math.Round(g.Sum(r => Math.Abs(r.MonthlyPositiveSum ?? 0)), 2));

        var negativeValues = records.Select(g =>
            Math.Round(g.Sum(r => Math.Abs(r.MonthlyNegativeSum ?? 0)), 2));

        var primarySolidColorPaint = hexadecimalCodePrimaryColor.ToSolidColorPaint();
        var secondarySolidColorPaint = hexadecimalCodeSecondaryColor.ToSolidColorPaint();

        var symbol = records.First().Select(s => s.Currency).First()!;
        var funcPositive = symbol.CreateLabelFunc();
        var funcNegative = symbol.CreateLabelFunc(-1);

        var positiveSeries = Commons.CreateColumnSeries(positiveName, positiveValues, primarySolidColorPaint, funcPositive);
        var negativeSeries = Commons.CreateColumnSeries(negativeName, negativeValues, secondarySolidColorPaint, funcNegative);

        return (positiveSeries, negativeSeries);
    }

    /// <summary>
    /// Creates a label formatting function for chart points using the specified currency symbol and multiplier.
    /// </summary>
    /// <param name="symbol">The currency symbol to be included in the formatted label.</param>
    /// <param name="multiplier">An optional multiplier to adjust the chart point value (default is 1).</param>
    /// <returns>A function that formats chart point labels as strings, including the currency symbol.</returns>
    private static Func<ChartPoint<double, RoundedRectangleGeometry, LabelGeometry>, string> CreateLabelFunc(
        this string symbol, int multiplier = 1)
        => point => $"{point.Model * multiplier:F2} {symbol}";
}