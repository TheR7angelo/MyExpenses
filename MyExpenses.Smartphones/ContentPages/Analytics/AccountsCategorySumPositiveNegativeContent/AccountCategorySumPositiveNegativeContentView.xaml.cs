using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Models.Ui;
using MyExpenses.Share.Core.Analysis;
using MyExpenses.SharedUtils.Converters;
using MyExpenses.SharedUtils.Resources.Resx.AccountsCategorySumPositiveNegativeContent;
using MyExpenses.Sql.Queries;
using MyExpenses.Utils;
using SkiaSharp;

namespace MyExpenses.Smartphones.ContentPages.Analytics.AccountsCategorySumPositiveNegativeContent;

public partial class AccountCategorySumPositiveNegativeContentView
{
    public static readonly BindableProperty TextPaintProperty = BindableProperty.Create(nameof(TextPaint),
        typeof(SolidColorPaint), typeof(AccountCategorySumPositiveNegativeContentView));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ObservableCollection<ISeries> Series { get; set; } = [];
    public ICartesianAxis[] XAxis { get; set; } = [];
    public ICartesianAxis[] YAxis { get; set; } = [];

    private int AccountId { get; set; } = 1;

    public AccountCategorySumPositiveNegativeContentView()
    {
        InitializeComponent();

        // ReSharper disable HeapView.DelegateAllocation
        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += UpdateLanguage;
        // ReSharper restore HeapView.DelegateAllocation
    }

    private void Interface_OnThemeChanged()
    {
        UpdateTextPaint();
        UpdateAxisTextPaint();
    }

    private void UpdateTextPaint()
    {
        var theme = AppInfo.RequestedTheme;
        var skColor = theme is AppTheme.Dark ? SKColors.White : SKColors.Black;
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

        for (var i = 0; i < Series.Count; i++)
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
            tmp!.Labels = tmp.Labels!.ToRoundTripDateLabelTransformation();
            XAxis[i] = tmp;
        }

        Span<string> names =
        [AccountsCategorySumPositiveNegativeContentResources.ColumnSeriesNegativeName,
            AccountsCategorySumPositiveNegativeContentResources.ColumnSeriesPositiveName];

        if (!Series.Count.Equals(names.Length)) return;

        for (var i = 0; i < names.Length; i++)
        {
            Series[i].Name = names[i];
        }

        // UpdateLayout();
    }

    public void SetChart()
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
        var axis = TextPaint.CreateAxis();

        YAxis = [axis];
        CartesianChart.YAxes = YAxis;
    }

    private void SetXAxis(IEnumerable<string> labels)
    {
        var transformedLabels = labels.ToTransformLabelsToTitleCaseDateFormat();
        var axis = transformedLabels.CreateAxis(TextPaint);

        XAxis = [axis];
        CartesianChart.XAxes = XAxis;
    }

    private void SetSeries(List<IGrouping<string?, AnalysisVAccountCategoryMonthlySumPositiveNegative>> records)
    {
        var configuration = Config.Configuration;
        var primarySolidColorPaint = configuration.Interface.Theme.HexadecimalCodePrimaryColor;
        var secondarySolidColorPaint = configuration.Interface.Theme.HexadecimalCodeSecondaryColor;

        var (positiveSeries, negativeSeries) = records.GenerateSeries(primarySolidColorPaint, secondarySolidColorPaint,
            AccountsCategorySumPositiveNegativeContentResources.ColumnSeriesPositiveName,
            AccountsCategorySumPositiveNegativeContentResources.ColumnSeriesNegativeName);

        Series.Clear();
        Series.Add(negativeSeries);
        Series.Add(positiveSeries);
    }

    private void AccountCategorySumPositiveNegativeContentView_OnBindingContextChanged(object? sender, EventArgs e)
    {
        if (BindingContext is not TabItemData tabItemData) return;

        AccountId = (int)tabItemData.Id!;
        UpdateTextPaint();

        SetChart();
        UpdateLanguage();
    }
}