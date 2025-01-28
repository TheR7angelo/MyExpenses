using LiveChartsCore.SkiaSharpView;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Utils;

namespace MyExpenses.Share.Core.Analysis;

public static class VAccountCategoryMonthlySumPositiveNegative
{
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

        var positiveSeries = Commons.CreateColumnSeries(positiveName,
            positiveValues, primarySolidColorPaint, y => $"{y.Model:F2} {symbol}");
        var negativeSeries = Commons.CreateColumnSeries(negativeName,
            negativeValues, secondarySolidColorPaint, y => $"{-1 * y.Model:F2} {symbol}");

        return (positiveSeries, negativeSeries);
    }
}