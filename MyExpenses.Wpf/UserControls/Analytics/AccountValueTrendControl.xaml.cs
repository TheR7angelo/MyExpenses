using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;
using SkiaSharp.Views.WPF;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class AccountValueTrendControl
{
    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    public SolidColorPaint TextPaint { get; }

    private List<Button> Buttons { get; set; } = [];

    public AccountValueTrendControl()
    {
        // TODO add listener color change
        var brush = (SolidColorBrush)FindResource("MaterialDesignBody");
        var wpfColor = brush.Color;
        TextPaint = new SolidColorPaint(wpfColor.ToSKColor());

        SetChart();

        InitializeComponent();

        SetButtonPanel();
    }

    private void SetButtonPanel()
        => Buttons.ForEach(s => ButtonPanel.Children.Add(s));

    private void SetChart()
    {
        using var context = new DataBaseContext();
        var groupsVAccountMonthlyCumulativeSums = context.VAccountMonthlyCumulativeSums
            .ToList()
            .GroupBy(s => s.Account)
            .ToList();

        var axis = groupsVAccountMonthlyCumulativeSums.First().Select(s => s.Period!);

        SetSeries(groupsVAccountMonthlyCumulativeSums);
        SetXAxis(axis);
        SetYAxis();
    }

    private void SetYAxis()
    {
        var axis = new Axis
        {
            LabelsPaint = TextPaint
        };
        YAxis = [axis];
    }

    private void SetXAxis(IEnumerable<string> labels)
    {
        var transformedLabels = labels.Select(label =>
        {
            var labelSplit = label.Split('-');

            if (!int.TryParse(labelSplit[0], out var year) || !int.TryParse(labelSplit[1], out var month))
                throw new FormatException($"Invalid format for label '{label}'. Expected 'YYYY-MM' format.");

            var d = new DateOnly(year, month, 1);
            var newLabel = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.ToString("Y"));

            return newLabel;
        }).ToList();

        var axis = new Axis
        {
            Labels = transformedLabels,
            LabelsPaint = TextPaint
        };
        XAxis = [axis];
    }

    private void SetSeries(List<IGrouping<string?, VAccountMonthlyCumulativeSum>> groupsVAccountMonthlyCumulativeSums)
    {
        var series = new List<ISeries>();
        foreach (var groupVAccountMonthlyCumulativeSums in groupsVAccountMonthlyCumulativeSums)
        {
            var name = groupVAccountMonthlyCumulativeSums.Key;
            var values = groupVAccountMonthlyCumulativeSums.Select(s => (double)s.CumulativeSum!);

            var lineSeries = new LineSeries<double>
            {
                Name = name,
                Values = values,
                Fill = null
            };

            series.Add(lineSeries);

            var button = new Button
            {
                Content = name,
                Margin = new Thickness(5)
            };
            button.Click += (_, _) => { lineSeries.IsVisible = !lineSeries.IsVisible; };

            Buttons.Add(button);
        }

        Series = series.ToArray();
    }
}