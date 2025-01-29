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
    public static ColumnSeries<T> CreateColumnSeries<T>(this string name, IEnumerable<T> values,
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

    /// <summary>
    /// Creates a new axis with the specified labels and label painting properties.
    /// </summary>
    /// <param name="labels">The collection of labels to display on the axis. Can be null if no labels are specified.</param>
    /// <param name="labelPaint">The paint settings used for rendering the labels.</param>
    /// <returns>A new instance of <see cref="Axis" /> configured with the provided labels and paint settings.</returns>
    public static Axis CreateAxis(this IList<string>? labels, SolidColorPaint labelPaint)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This allocation is required to define a custom axis.
        var axis = new Axis
        {
            Labels = labels,
            LabelsPaint = labelPaint
        };

        return axis;
    }

    /// <summary>
    /// Creates a new axis with the specified label painting properties and optional labels.
    /// </summary>
    /// <param name="labelPaint">The paint settings used for rendering the labels on the axis.</param>
    /// <returns>A new instance of <see cref="Axis" /> configured with the provided paint settings.</returns>
    public static Axis CreateAxis(this SolidColorPaint labelPaint)
        => CreateAxis(null, labelPaint);
}