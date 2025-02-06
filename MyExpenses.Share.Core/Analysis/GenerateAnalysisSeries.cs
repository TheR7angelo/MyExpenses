using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Bases.Groups.VAccountCategoryMonthlySums;
using MyExpenses.Models.Sql.Bases.Groups.VAccountModePaymentCategoryMonthlySums;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.SharedUtils.Maths;
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
            var name = groupsByAccount.First().Account!;
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

            // ReSharper disable once HeapView.DelegateAllocation
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
    /// Generates a collection of series representing annual budget period analysis for the given grouped account records.
    /// </summary>
    /// <param name="recordsByAccounts">The collection of records grouped by account, containing budget period data.</param>
    /// <param name="trend">The label used to describe the trend series.</param>
    /// <param name="currency">The currency symbol used for displaying values.</param>
    /// <returns>An enumerable collection of series representing the data and its calculated trend line.</returns>
    public static IEnumerable<ISeries> GenerateSeries(
        this IEnumerable<IGrouping<int?, AnalysisVBudgetPeriodAnnual>> recordsByAccounts,
        string trend, string currency)
    {
        foreach (var recordsByAccount in recordsByAccounts)
        {
            var name = recordsByAccount.First().AccountName!;

            var values = recordsByAccount.Select(s => Math.Round(s.PeriodValue ?? 0, 2)).ToList();
            var analysisVBudgetMonthlies = recordsByAccount.Select(s => Mapping.Mapper.Map<BudgetRecordInfo>(s)).ToArray();

            var func = analysisVBudgetMonthlies.CreateCircleGeometryLabelFunc(currency);
            var lineSeries = name.CreateLineSeries(values, func);

            yield return lineSeries;

            var trendValues = values.GenerateLinearTrendValues();

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var isSeriesTranslatableTrend = new IsSeriesTranslatable { OriginalName = name, IsTranslatable = true };

            var trendName = $"{name} {trend}";
            func = currency.CreateCircleGeometryLabelFunc();
            var trendSeries = trendName.CreateLineSeries(trendValues, func, null, false, 0, isSeriesTranslatableTrend);

            yield return trendSeries;
        }
    }

    /// <summary>
    /// Generates a collection of series (ISeries) from grouped records,
    /// including both normal and trend series.
    /// </summary>
    /// <param name="recordsByAccounts">Grouped records, each group represents an account.</param>
    /// <param name="trend">Suffix for trend series names (e.g., "Trend").</param>
    /// <param name="currency">Currency symbol for data point labels (e.g., "$").</param>
    /// <returns>
    /// An IEnumerable of ISeries containing one normal and one trend series per group.
    /// </returns>
    public static IEnumerable<ISeries> GenerateSeries(
        this IEnumerable<IGrouping<int?, AnalysisVBudgetMonthly>> recordsByAccounts,
        string trend, string currency)
    {
        foreach (var recordsByAccount in recordsByAccounts)
        {
            var name = recordsByAccount.First().AccountName!;

            var values = recordsByAccount.Select(s => Math.Round(s.PeriodValue ?? 0, 2)).ToList();

            var analysisVBudgetMonthlies = recordsByAccount.Select(s => Mapping.Mapper.Map<BudgetRecordInfo>(s)).ToArray();
            var func = analysisVBudgetMonthlies.CreateCircleGeometryLabelFunc(currency);

            var lineSeries = name.CreateLineSeries(values, func);

            yield return lineSeries;

            var trendValues = values.GenerateLinearTrendValues();

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var isSeriesTranslatableTrend = new IsSeriesTranslatable { OriginalName = name, IsTranslatable = true };

            var trendName = $"{name} {trend}";
            func = currency.CreateCircleGeometryLabelFunc();
            var trendSeries = trendName.CreateLineSeries(trendValues, func, null, false, 0, isSeriesTranslatableTrend);

            yield return trendSeries;
        }
    }

    /// <summary>
    /// Generates a collection of series for the given grouped annual budget records based on account data.
    /// </summary>
    /// <param name="recordsByAccounts">A collection of grouped annual budget records by account.</param>
    /// <param name="trend">The trend label to be applied to the trend series.</param>
    /// <param name="currency">The currency symbol to format the data points and tooltips in the series.</param>
    /// <returns>An enumerable collection of series, including line series for the data and its corresponding trend.</returns>
    public static IEnumerable<ISeries> GenerateSeries(
        this IEnumerable<IGrouping<int?, AnalysisVBudgetTotalAnnual>> recordsByAccounts,
        string trend, string currency)
    {
        foreach (var recordsByAccount in recordsByAccounts)
        {
            var name = recordsByAccount.First().AccountName!;
            var values = recordsByAccount.Select(s => Math.Round(s.PeriodValue ?? 0, 2)).ToList();

            var analysisVBudgetMonthlies = recordsByAccount.Select(s => Mapping.Mapper.Map<BudgetRecordInfo>(s)).ToArray();

            var func = analysisVBudgetMonthlies.CreateCircleGeometryLabelFunc(currency);
            var lineSeries = name.CreateLineSeries(values, func);

            yield return lineSeries;

            var trendValues = values.GenerateLinearTrendValues();

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var isSeriesTranslatableTrend = new IsSeriesTranslatable { OriginalName = name, IsTranslatable = true };

            var trendName = $"{name} {trend}";
            func = currency.CreateCircleGeometryLabelFunc();
            var trendSeries = trendName.CreateLineSeries(trendValues, func, null, false, 0, isSeriesTranslatableTrend);

            yield return trendSeries;
        }
    }

    /// <summary>
    /// Creates a label formatting function for chart points using the specified currency symbol and multiplier.
    /// </summary>
    /// <param name="symbol">The currency symbol to be included in the formatted label.</param>
    /// <param name="multiplier">An optional multiplier to adjust the chart point value (default is 1).</param>
    /// <returns>A function that formats chart point labels as strings, including the currency symbol.</returns>
    private static Func<ChartPoint<double, RoundedRectangleGeometry, LabelGeometry>, string> CreateRoundedRectangleLabelFunc(
        this string symbol, int multiplier = 1)
        // ReSharper disable once HeapView.DelegateAllocation
        => point => $"{point.Model * multiplier:F2} {symbol}";

    /// <summary>
    /// Creates a function to format the labels for the doughnut chart, using a specified symbol and multiplier.
    /// </summary>
    /// <param name="symbol">The symbol to append to the label (e.g., currency symbol).</param>
    /// <param name="multiplier">The multiplier to adjust the chart point's value before formatting.</param>
    /// <returns>A function that takes a chart point and returns a formatted string label.</returns>
    public static Func<ChartPoint<double, DoughnutGeometry, LabelGeometry>, string> CreateDoughnutLabelFunc(
            this string symbol, int multiplier)
        // ReSharper disable once HeapView.DelegateAllocation
        => point => $"{point.Model * multiplier:F2} {symbol}";

    /// <summary>
    /// Creates a function that generates a label text for circle geometries based on the budget records and currency.
    /// </summary>
    /// <param name="records">An array of budget record information used for label generation.</param>
    /// <param name="currency">The currency symbol to include in the label text.</param>
    /// <returns>A function that takes a chart point and returns the corresponding label as a string.</returns>
    private static Func<ChartPoint<double, CircleGeometry, LabelGeometry>, string> CreateCircleGeometryLabelFunc
        (this BudgetRecordInfo[] records, string currency)
        // ReSharper disable once HeapView.DelegateAllocation
        => point =>
        {
            var dataPoint = records[point.Index];
            return $"{dataPoint.Value} {currency}{Environment.NewLine}" +
                   $"{dataPoint.Status} {dataPoint.DifferenceValue:F2}{currency} ({dataPoint.Percentage}%)";
        };
    
    /// <summary>
    /// Creates a label formatting function for circle geometry labels, displaying the provided symbol alongside the numeric value.
    /// </summary>
    /// <param name="symbol">The symbol to be appended to the numeric value in the label.</param>
    /// <returns>A function that formats a string label for a chart point with a numeric model and the given symbol.</returns>
    private static Func<ChartPoint<double, CircleGeometry, LabelGeometry>, string> CreateCircleGeometryLabelFunc(this string symbol)
        // ReSharper disable once HeapView.DelegateAllocation
        => point => $"{point.Model:F2} {symbol}";
}