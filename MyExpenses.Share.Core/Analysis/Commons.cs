using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;

namespace MyExpenses.Share.Core.Analysis;

public static class Commons
{
    /// <summary>
    /// Creates a new column series with the specified properties.
    /// </summary>
    /// <typeparam name="T">The type of the values in the column series.</typeparam>
    /// <param name="name">The name of the column series.</param>
    /// <param name="values">The collection of values to be displayed in the column series.</param>
    /// <param name="solidColorPaint">The solid color paint used to fill the columns in the series. Null if no color is specified.</param>
    /// <param name="tooltipFormatter">An optional function to format the tooltip labels associated with the series data points.</param>
    /// <returns>A new instance of <see cref="ColumnSeries{T}" /> configured with the provided properties.</returns>
    public static ColumnSeries<T> CreateColumnSeries<T>(string name, IEnumerable<T> values,
        SolidColorPaint? solidColorPaint,
        Func<ChartPoint<T, RoundedRectangleGeometry, LabelGeometry>, string>? tooltipFormatter)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This allocation is required to define a custom column series (ColumnSeries<T>).
        return new ColumnSeries<T>
        {
            Name = name,
            Values = values.ToList(),
            Fill = solidColorPaint,
            YToolTipLabelFormatter = tooltipFormatter
        };
    }
}