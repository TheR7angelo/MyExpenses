using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
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
    /// <param name="fill">The fill paint to apply to the columns in the series.</param>
    /// <param name="tooltipFormatter">An optional function to format the tooltip labels associated with the series data points.</param>
    /// <param name="dataLabelFormatter">An optional function to format the displayed data labels for the series data points.</param>
    /// <param name="dataLabelsPaint">An optional paint to style the data labels.</param>
    /// <param name="dataLabelsPosition">The position of the data labels in relation to the columns in the series.</param>
    /// <returns>A new instance of <see cref="ColumnSeries{T}" /> configured with the provided properties.</returns>
    public static ColumnSeries<T> CreateColumnSeries<T>(this string name, IEnumerable<T> values,
        SolidColorPaint? fill,
        Func<ChartPoint<T, RoundedRectangleGeometry, LabelGeometry>, string>? tooltipFormatter = null,
        Func<ChartPoint<T, RoundedRectangleGeometry, LabelGeometry>, string>? dataLabelFormatter = null,
        SolidColorPaint? dataLabelsPaint = null, DataLabelsPosition? dataLabelsPosition = null)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This allocation is required to define a custom column series (ColumnSeries<T>).
        var columnSeries  = new ColumnSeries<T>
        {
            Name = name,
            Values = values.ToList()
        };

        if (fill is not null) columnSeries.Fill = fill;
        if (tooltipFormatter is not null) columnSeries.YToolTipLabelFormatter = tooltipFormatter;
        if (dataLabelFormatter is not null) columnSeries.DataLabelsFormatter = dataLabelFormatter;
        if (dataLabelsPaint is not null) columnSeries.DataLabelsPaint = dataLabelsPaint;
        if (dataLabelsPosition is not null) columnSeries.DataLabelsPosition = (DataLabelsPosition)dataLabelsPosition;

        return columnSeries;
    }

    /// <summary>
    /// Creates a new stacked column series with the specified properties.
    /// </summary>
    /// <typeparam name="T">The type of the values in the stacked column series.</typeparam>
    /// <param name="name">The name of the stacked column series.</param>
    /// <param name="values">The collection of values to be displayed in the stacked column series.</param>
    /// <param name="tooltipFormatter">An optional function to format the tooltip labels associated with the series data points.</param>
    /// <returns>A new instance of <see cref="StackedColumnSeries{T}" /> configured with the provided properties.</returns>
    public static StackedColumnSeries<T> CreateStackedColumnSeries<T>(this string name, IEnumerable<T> values,
        Func<ChartPoint<T, RoundedRectangleGeometry, LabelGeometry>, string>? tooltipFormatter)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This allocation is required to define a custom column series (ColumnSeries<T>).
        return new StackedColumnSeries<T>
        {
            Name = name,
            Values = values.ToList(),
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