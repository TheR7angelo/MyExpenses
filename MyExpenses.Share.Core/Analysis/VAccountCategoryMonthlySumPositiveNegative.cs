using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Utils;

namespace MyExpenses.Share.Core.Analysis;

public static class VAccountCategoryMonthlySumPositiveNegative
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
        var symbol = records.First().Select(s => s.Currency).First();

        var positiveValues = records.Select(g =>
            Math.Round(g.Sum(r => Math.Abs(r.MonthlyPositiveSum ?? 0)), 2));

        var negativeValues = records.Select(g =>
            Math.Round(g.Sum(r => Math.Abs(r.MonthlyNegativeSum ?? 0)), 2));

        var primarySolidColorPaint = hexadecimalCodePrimaryColor.ToSolidColorPaint();
        var secondarySolidColorPaint = hexadecimalCodeSecondaryColor.ToSolidColorPaint();

        var funcPositive = new Func<ChartPoint<double, RoundedRectangleGeometry, LabelGeometry>, string>(y => $"{y.Model:F2} {symbol}");
        var funcNegative = new Func<ChartPoint<double, RoundedRectangleGeometry, LabelGeometry>, string>(y => $"{-1 * y.Model:F2} {symbol}");

        var positiveSeries = Commons.CreateColumnSeries(positiveName, positiveValues, primarySolidColorPaint, funcPositive);
        var negativeSeries = Commons.CreateColumnSeries(negativeName, negativeValues, secondarySolidColorPaint, funcNegative);

        return (positiveSeries, negativeSeries);
    }
}