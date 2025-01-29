using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Sql.Bases.Groups.VAccountCategoryMonthlySums;
using MyExpenses.Models.Sql.Bases.Groups.VAccountModePaymentCategoryMonthlySums;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Models.Wpf.Charts;
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
        var funcPositive = symbol.CreateRoundedRectangleLabelFunc();
        var funcNegative = symbol.CreateRoundedRectangleLabelFunc(-1);

        var positiveSeries = positiveName.CreateColumnSeries(positiveValues, primarySolidColorPaint, funcPositive);
        var negativeSeries = negativeName.CreateColumnSeries(negativeValues, secondarySolidColorPaint, funcNegative);

        return (positiveSeries, negativeSeries);
    }

    /// <summary>
    /// Generates a list of series for grouped account categories, with each series representing the corresponding category group.
    /// </summary>
    /// <param name="groupsByCategories">A list of groups categorized by account category, each containing monthly sums and other metadata.</param>
    /// <returns>A list of series representing the grouped account categories, with their respective values and visual styling.</returns>
    public static IEnumerable<ColumnSeries<double>> GenerateSeries(this List<IGrouping<string?, GroupsByCategories>> groupsByCategories)
    {
        var currency = groupsByCategories.First().Select(s => s.Currency).First()!;
        var func = currency.CreateRoundedRectangleLabelFunc();

        foreach (var groupsByCategory in groupsByCategories)
        {
            var name = groupsByCategory.Key!;
            var colorCode = groupsByCategory.First().ColorCode;
            var solidColorPaint = colorCode.ToSolidColorPaint();
            var values = groupsByCategory.Select(s => Math.Round(s.SumMonthlySum ?? 0, 2));

            var columnSeries = name.CreateColumnSeries(values, solidColorPaint, func);

            yield return columnSeries;
        }
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
    public static IEnumerable<StackedColumnSeries<double>> GenerateSeries(
        this List<IGrouping<int?, AnalysisVAccountMonthlyCumulativeSum>> groupsByAccounts)
    {
        var currency = groupsByAccounts.First().Select(s => s.Currency).First()!;
        var func = currency.CreateRoundedRectangleLabelFunc();

        foreach (var groupsByAccount in groupsByAccounts)
        {
            var name = groupsByAccount.Key!.ToString()!;
            var values = groupsByAccount.Select(s => Math.Round(s.CumulativeSum ?? 0, 2)).ToList();

            var stakedColumnSeries = name.CreateStackedColumnSeries(values, func);

            yield return stakedColumnSeries;
        }
    }

    /// <summary>
    /// Generates a collection of column series based on the given groups of mode payment categories and a specified text paint format.
    /// </summary>
    /// <param name="groupsByModePayments">The grouped data containing mode payment categories and related monthly sums.</param>
    /// <param name="textPaint">The text paint used for data labels formatting in the series.</param>
    /// <returns>A collection of column series with formatted data labels based on the groups provided.</returns>
    public static IEnumerable<ColumnSeries<double>> GenerateSeries(
        this List<IGrouping<string?, GroupsByModePaymentCategory>> groupsByModePayments, SolidColorPaint textPaint)
    {
        var currency = groupsByModePayments.First().Select(s => s.Currency).First()!;

        var tooltipFormatter = currency.CreateRoundedRectangleLabelFunc();

        foreach (var groupsByCategory in groupsByModePayments)
        {
            var name = groupsByCategory.Key!;

            var monthlyPaymentDataPoints = groupsByCategory
                .GroupBy(s => s.Period)
                // ReSharper disable once HeapView.ObjectAllocation.Evident
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

            yield return columnSeries;
        }
    }

    /// <summary>
    /// Generates a line series and its corresponding trend series based on the provided records and parameters.
    /// </summary>
    /// <param name="recordsByAccount">The grouped records by account containing periodic budget data.</param>
    /// <param name="name">The name/label for the generated line series.</param>
    /// <param name="trendName">The name/label for the generated trend series.</param>
    /// <param name="currency">The currency symbol to be used in tooltips and labels.</param>
    /// <returns>A tuple containing the line series and its corresponding trend series.</returns>
    public static (LineSeries<double> LineSeries, LineSeries<double> TrendSeries) GenerateSeries(
        this IGrouping<int?, AnalysisVBudgetPeriodAnnual> recordsByAccount, string name, string trendName,
        string currency)
    {
        var values = recordsByAccount.Select(s => Math.Round(s.PeriodValue ?? 0, 2)).ToList();

        var analysisVBudgetMonthlies = recordsByAccount.Select(s => s)
            .ToList();

        var lineSeries = name.CreateLineSeries(values, point =>
        {
            var dataPoint = analysisVBudgetMonthlies[point.Index];
            return $"{dataPoint.PeriodValue} {currency}{Environment.NewLine}" +
                   $"{dataPoint.Status} {dataPoint.DifferenceValue ?? 0:F2}{currency} ({dataPoint.Percentage}%)";
        });

        var xData = Enumerable.Range(1, values.Count).Select(i => (double)i).ToArray();
        var (a, b) = AnalyticsUtils.CalculateLinearTrend(xData, values.ToArray());
        var trendValues = xData.Select(x => Math.Round(a * x + b, 2)).ToArray();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This allocation is required to define a custom line series (LineSeries<double>).
        var isSeriesTranslatableTrend = new IsSeriesTranslatable { OriginalName = name, IsTranslatable = true };

        var func = currency.CreateCircleGeometryLabelFunc();
        var trendSeries = trendName.CreateLineSeries(trendValues, func, null, false, 0, isSeriesTranslatableTrend);

        return (lineSeries, trendSeries);
    }

    /// <summary>
    /// Creates a label formatting function for chart points using the specified currency symbol and multiplier.
    /// </summary>
    /// <param name="symbol">The currency symbol to be included in the formatted label.</param>
    /// <param name="multiplier">An optional multiplier to adjust the chart point value (default is 1).</param>
    /// <returns>A function that formats chart point labels as strings, including the currency symbol.</returns>
    private static Func<ChartPoint<double, RoundedRectangleGeometry, LabelGeometry>, string> CreateRoundedRectangleLabelFunc(
        this string symbol, int multiplier = 1)
        => point => $"{point.Model * multiplier:F2} {symbol}";

    private static Func<ChartPoint<double, CircleGeometry, LabelGeometry>, string> CreateCircleGeometryLabelFunc(this string symbol)
        => point => $"{point.Model:F2} {symbol}";
}