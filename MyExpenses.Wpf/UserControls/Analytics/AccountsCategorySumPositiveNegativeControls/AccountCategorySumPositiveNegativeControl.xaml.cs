using System.Windows;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Share.Core.Analysis;
using MyExpenses.Sql.Queries;
using MyExpenses.Utils;
using MyExpenses.Wpf.Converters.Analytics;
using MyExpenses.Wpf.Resources.Resx.UserControls.Analytics.AccountsCategorySumPositiveNegativeControls;

namespace MyExpenses.Wpf.UserControls.Analytics.AccountsCategorySumPositiveNegativeControls;

public partial class AccountCategorySumPositiveNegativeControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(SolidColorPaint), typeof(AccountCategorySumPositiveNegativeControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    private int AccountId { get; }

    public AccountCategorySumPositiveNegativeControl(int accountId)
    {
        AccountId = accountId;

        UpdateTextPaint();

        SetChart();
        UpdateLanguage();

        InitializeComponent();

        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void Interface_OnThemeChanged()
    {
        UpdateTextPaint();
        UpdateAxisTextPaint();
    }

    private void UpdateTextPaint()
    {
        var skColor = Utils.Resources.GetMaterialDesignBodySkColor();
        TextPaint = skColor.ToSolidColorPaint();
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

        var configuration = Config.Configuration;
        var primarySolidColorPaint = configuration.Interface.Theme.HexadecimalCodePrimaryColor.ToSolidColorPaint();
        var secondarySolidColorPaint = configuration.Interface.Theme.HexadecimalCodeSecondaryColor.ToSolidColorPaint();

        Span<SolidColorPaint?> solidColorPaints = [secondarySolidColorPaint, primarySolidColorPaint];

        for (var i = 0; i < Series.Length; i++)
        {
            var tmp = Series[i] as ColumnSeries<double>;
            tmp!.Fill = solidColorPaints[i];
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

        Span<string> names =
            [AccountsCategorySumPositiveNegativeControlsResources.ColumnSeriesNegativeName,
            AccountsCategorySumPositiveNegativeControlsResources.ColumnSeriesPositiveName];

        for (var i = 0; i < names.Length; i++)
        {
            Series[i].Name = names[i];
        }

        UpdateLayout();
    }

    private void SetChart()
    {
        var records = AccountId.GetVAccountCategoryMonthlySumPositiveNegative();
        
        if (records.Count is 0) return;

        var axis = records.Select(s => s.Key!);

        SetSeries(records);
        SetXAxis(axis);
        SetYAxis();
    }

    private void SetYAxis()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This allocation is required to define a custom vertical axis (YAxis).
        // We configure the axis here with a specific paint (TextPaint) to ensure
        // the label style aligns with the application's visual theme.
        // This method ensures the chart always has a properly configured Y-axis.
        var axis = new Axis
        {
            LabelsPaint = TextPaint
        };
        YAxis = [axis];
    }

    private void SetXAxis(IEnumerable<string> labels)
    {
        var transformedLabels = labels.ToTransformLabelsToTitleCaseDateFormat();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This allocation is required to define a custom horizontal axis (XAxis).
        // The axis is configured with transformed labels (transformedLabels)
        // and a specific paint (TextPaint) to ensure both proper labeling and
        // a consistent style aligned with the application's visual theme.
        var axis = new Axis
        {
            Labels = transformedLabels,
            LabelsPaint = TextPaint
        };
        XAxis = [axis];
    }

    private void SetSeries(List<IGrouping<string?, AnalysisVAccountCategoryMonthlySumPositiveNegative>> records)
    {
        var configuration = Config.Configuration;
        var primarySolidColorPaint = configuration.Interface.Theme.HexadecimalCodePrimaryColor;
        var secondarySolidColorPaint = configuration.Interface.Theme.HexadecimalCodeSecondaryColor;

        var (positiveSeries, negativeSeries) = records.GenerateSeries(primarySolidColorPaint, secondarySolidColorPaint,
            AccountsCategorySumPositiveNegativeControlsResources.ColumnSeriesPositiveName,
            AccountsCategorySumPositiveNegativeControlsResources.ColumnSeriesNegativeName);

        Series = [negativeSeries, positiveSeries];
    }

}