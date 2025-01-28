using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;

namespace MyExpenses.Share.Core.Analysis;

public static class Commons
{
    public static ColumnSeries<T> CreateColumnSeries<T>(string name, IEnumerable<T> values, SolidColorPaint? solidColorPaint,
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