using System.Windows.Data;
using LiveChartsCore;
using MyExpenses.Models.Wpf.Charts;

namespace MyExpenses.Wpf.UserControls.Analytics.BudgetsControl;

public static class FilterSeriesTrend
{
    /// <summary>
    /// Filters items in the collection to determine whether they should be included in the view source for series.
    /// </summary>
    /// <param name="e">The event arguments containing details about the item to be filtered, as well as the result of the filter operation.</param>
    public static void ViewSourceSeries_OnFilter(FilterEventArgs e)
    {
        if (e.Item is not ISeries series) e.Accepted = false;
        else
        {
            e.Accepted = series.Tag switch
            {
                null => true,
                IsSeriesTranslatable { IsTrend: false } => true,
                _ => false
            };
        }
    }

    /// <summary>
    /// Filters items in the collection to determine whether they should be included in the view source for trend series.
    /// </summary>
    /// <param name="e">The event arguments containing details about the item to be filtered, as well as the result of the filter operation.</param>
    public static void ViewSourceTrendSeries_OnFilter(FilterEventArgs e)
    {
        if (e.Item is not ISeries series) e.Accepted = false;
        else
        {
            e.Accepted = series.Tag switch
            {
                null => false,
                IsSeriesTranslatable { IsTrend: true } => true,
                _ => false
            };
        }
    }
}