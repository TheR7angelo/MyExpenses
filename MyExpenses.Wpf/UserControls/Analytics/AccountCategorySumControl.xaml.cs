using System.Windows;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Converters.Analytics;
using MyExpenses.Wpf.Utils;
using SkiaSharp;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class AccountCategorySumControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        typeof(SolidColorPaint), typeof(AccountCategorySumControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    private int AccountId { get; }

    public AccountCategorySumControl(int accountId)
    {
        AccountId = accountId;

        var skColor = Utils.Resources.GetSkColor();
        TextPaint = new SolidColorPaint(skColor);

        SetChart();
        UpdateLanguage();

        InitializeComponent();

        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void Interface_OnThemeChanged(object sender, ConfigurationThemeChangedEventArgs e)
    {
        var skColor = MyExpenses.Wpf.Utils.Resources.GetSkColor();
        TextPaint = new SolidColorPaint(skColor);

        UpdateAxisTextPaint();
    }

    #endregion

    #region Function

    private void SetChart()
    {
        using var context = new DataBaseContext();
        var groupsByCategories = context.VAccountCategoryMonthlyCumulativeSums
            .Where(s => s.AccountFk == AccountId)
            .OrderBy(s => s.Period).ThenBy(s => s.CategoryType)
            .ToList().GroupBy(s => s.CategoryType).ToList();

        if (groupsByCategories.Count is 0) return;

        var axis = groupsByCategories.First().Select(s => s.Period!);

        SetSeries(groupsByCategories);
        SetXAxis(axis);
        SetYAxis();
    }

    private void SetSeries(List<IGrouping<string?, VAccountCategoryMonthlySum>> groupsByCategories)
    {

        var series = new List<ISeries>();

        foreach (var groupsByCategory in groupsByCategories)
        {
            var name = groupsByCategory.Key;
            var colorCode = groupsByCategory.First().ColorCode;
            var skColor = (SKColor)colorCode!.ToSkColor()!;
            var values = groupsByCategory.Select(s => Math.Round(s.MonthlySum ?? 0, 2));

            var columnSeries = new ColumnSeries<double>
            {
                Name = name,
                Fill = new SolidColorPaint(skColor),
                Values = values
            };

            series.Add(columnSeries);
        }
        Series = [..series];
    }

    private void SetXAxis(IEnumerable<string> labels)
    {
        var transformedLabels = labels.ToTransformLabelsToTitleCaseDateFormat();

        var axis = new Axis
        {
            Labels = transformedLabels,
            LabelsPaint = TextPaint
        };
        XAxis = [axis];
    }

    private void SetYAxis()
    {
        var axis = new Axis
        {
            LabelsPaint = TextPaint
        };
        YAxis = [axis];
    }

    private void UpdateAxisTextPaint()
    {
        for (var i = 0; i < YAxis.Length; i++)
        {
            var tmp = YAxis[i] as Axis;
            tmp!.LabelsPaint = TextPaint;
            YAxis[i] = tmp;
        }

        for (var i = 0; i < XAxis.Length; i++)
        {
            var tmp = XAxis[i] as Axis;
            tmp!.LabelsPaint = TextPaint;
            XAxis[i] = tmp;
        }
    }

    private void UpdateLanguage()
    {
        for (var i = 0; i < XAxis.Length; i++)
        {
            var tmp = XAxis[i] as Axis;
            tmp!.Labels = tmp.Labels!
                .ToTransformLabelsToTitleCaseDateFormatConvertBack()
                .ToTransformLabelsToTitleCaseDateFormat();
            XAxis[i] = tmp;
        }

        UpdateLayout();
    }

    #endregion
}