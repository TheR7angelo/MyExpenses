using System.Windows;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Converters.Analytics;
using MyExpenses.Wpf.Resources.Resx.UserControls.Analytics.AccountsCategorySumPositiveNegativeControls;
using MyExpenses.Wpf.Utils;
using SkiaSharp;

namespace MyExpenses.Wpf.UserControls.Analytics.BudgetsControl;

public partial class BudgetMonthlyControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        typeof(SolidColorPaint), typeof(BudgetMonthlyControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    private int AccountId { get; }
    public BudgetMonthlyControl(int accountId)
    {
        AccountId = accountId;

        var skColor = Utils.Resources.GetMaterialDesignBodySkColor();
        TextPaint = new SolidColorPaint(skColor);

        SetChart();
        UpdateLanguage();
        InitializeComponent();

        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void SetChart()
    {
        using var context = new DataBaseContext();
        var records = context.AnalysisVBudgetMonthlies
            .Where(s => s.AccountFk == AccountId)
            .AsEnumerable()
            .GroupBy(s => s.Period)
            .ToList();

        if (records.Count is 0) return;

        var axis = records.Select(s => s.Key!);

        // SetSeries(records);
        // SetXAxis(axis);
        // SetYAxis();
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void Interface_OnThemeChanged(object sender, ConfigurationThemeChangedEventArgs e)
    {
        var skColor = Utils.Resources.GetMaterialDesignBodySkColor();
        TextPaint = new SolidColorPaint(skColor);

        UpdateAxisTextPaint();
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

        var configuration = MyExpenses.Utils.Config.Configuration;
        var primaryColor = ((Color)configuration.Interface.Theme.HexadecimalCodePrimaryColor.ToColor()!).ToSkColor();
        var secondaryColor = ((Color)configuration.Interface.Theme.HexadecimalCodeSecondaryColor.ToColor()!).ToSkColor();
        var skColors = new List<SKColor> { secondaryColor, primaryColor };

        for (var i = 0; i < Series.Length; i++)
        {
            var tmp = Series[i] as ColumnSeries<double>;
            tmp!.Fill = new SolidColorPaint(skColors[i]);
            Series[i] = tmp;
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

        var names = new List<string>
        {
            AccountsCategorySumPositiveNegativeControlsResources.ColumnSeriesNegativeName,
            AccountsCategorySumPositiveNegativeControlsResources.ColumnSeriesPositiveName
        };

        for (var i = 0; i < names.Count; i++)
        {
            Series[i].Name = names[i];
        }

        UpdateLayout();
    }
}