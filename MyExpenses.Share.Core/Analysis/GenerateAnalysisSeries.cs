using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Sql.Bases.Groups.VAccountCategoryMonthlySums;
using MyExpenses.Models.Sql.Bases.Groups.VAccountModePaymentCategoryMonthlySums;
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

        var positiveSeries = positiveName.CreateColumnSeries(positiveValues, primarySolidColorPaint, funcPositive);
        var negativeSeries = negativeName.CreateColumnSeries(negativeValues, secondarySolidColorPaint, funcNegative);

        return (positiveSeries, negativeSeries);
    }

    /// <summary>
    /// Generates a list of series for grouped account categories, with each series representing the corresponding category group.
    /// </summary>
    /// <param name="groupsByCategories">A list of groups categorized by account category, each containing monthly sums and other metadata.</param>
    /// <returns>A list of series representing the grouped account categories, with their respective values and visual styling.</returns>
    public static List<ISeries> GenerateSeries(this List<IGrouping<string?, GroupsByCategories>> groupsByCategories)
    {
        var currency = groupsByCategories.First().Select(s => s.Currency).First()!;
        var func = currency.CreateLabelFunc();

        var series = new List<ISeries>();
        foreach (var groupsByCategory in groupsByCategories)
        {
            var name = groupsByCategory.Key!;
            var colorCode = groupsByCategory.First().ColorCode;
            var solidColorPaint = colorCode.ToSolidColorPaint();
            var values = groupsByCategory.Select(s => Math.Round(s.SumMonthlySum ?? 0, 2));

            var columnSeries = name.CreateColumnSeries(values, solidColorPaint, func);

            series.Add(columnSeries);
        }

        return series;
    }

    /// <summary>
    /// Generates a list of stacked column series for grouped account monthly cumulative sums.
    /// Each series represents an account group with its respective cumulative values and formatted labels.
    /// </summary>
    /// <param name="groupsByAccounts">
    /// A list of groups categorized by accounts, each containing cumulative sums and related account metadata.
    /// </param>
    /// <returns>
    /// A list of <see cref="ISeries"/> objects, each representing a stacked column series
    /// for the corresponding account group.
    /// </returns>
    public static List<ISeries> GenerateSeries(
        this List<IGrouping<int?, AnalysisVAccountMonthlyCumulativeSum>> groupsByAccounts)
    {
        var currency = groupsByAccounts.First().Select(s => s.Currency).First()!;
        var func = currency.CreateLabelFunc();

        var series = new List<ISeries>();
        foreach (var groupsByAccount in groupsByAccounts)
        {
            var values = groupsByAccount.Select(s => Math.Round(s.CumulativeSum ?? 0, 2)).ToList();

            var stakedColumnSeries = new StackedColumnSeries<double>
            {
                Values = values,
                Name = groupsByAccount.First().Account,
                YToolTipLabelFormatter = func
            };

            series.Add(stakedColumnSeries);
        }

        return series;
    }

    public static List<ISeries> GenerateSeries(this List<IGrouping<string?, GroupsByModePaymentCategory>> groupsByModePayments, SolidColorPaint textPaint)
    {
        var currency = groupsByModePayments.First().Select(s => s.Currency).First()!;

        var tooltipFormatter = currency.CreateLabelFunc();

        var series = new List<ISeries>();
        foreach (var groupsByCategory in groupsByModePayments)
        {
            var name = groupsByCategory.Key!;

            var monthlyPaymentDataPoints = groupsByCategory
                .GroupBy(s => s.Period)
                .Select(g => new
                {
                    MonthlySum = Math.Round(g.Sum(v => v.TotalMonthlySum ?? 0), 2),
                    MonthlyModePayment = g.Sum(v => v.TotalMonthlyModePayment)
                })
                .ToArray();

            var values = monthlyPaymentDataPoints.Select(s => s.MonthlySum).ToList();
            var columnSeries = name.CreateColumnSeries(values, null, tooltipFormatter, point =>
                {
                    var index = point.Index;
                    var dataPoint = monthlyPaymentDataPoints[index];
                    return dataPoint.MonthlyModePayment is 0 ? string.Empty : dataPoint.MonthlyModePayment.ToString()!;
                },
                textPaint,
                DataLabelsPosition.Middle
            );

            series.Add(columnSeries);
        }
        return series;
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