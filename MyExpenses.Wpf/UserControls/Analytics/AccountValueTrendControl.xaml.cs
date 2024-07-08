using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class AccountValueTrendControl
{
    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;

    public AccountValueTrendControl()
    {
        SetChart();

        InitializeComponent();
    }

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
    }

    private void SetXAxis(IEnumerable<string> labels)
    {
        var axis = new Axis { Labels = labels.ToList() };
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
        }

        Series = series.ToArray();
    }
}