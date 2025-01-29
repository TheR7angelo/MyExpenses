using System.Windows;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Groups.VAccountCategoryMonthlySums;
using MyExpenses.Share.Core.Analysis;
using MyExpenses.Sql.Queries;
using MyExpenses.Utils;
using MyExpenses.Wpf.Converters.Analytics;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class AccountCategorySumControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
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

        UpdateTextPaint();
        SetChart();
        UpdateLanguage();

        InitializeComponent();

        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void Interface_OnThemeChanged()
    {
        UpdateTextPaint();
        UpdateAxisTextPaint();
    }

    #endregion

    #region Function

    private void UpdateTextPaint()
    {
        var skColor = Utils.Resources.GetMaterialDesignBodySkColor();
        TextPaint = skColor.ToSolidColorPaint();
    }

    private void SetChart()
    {
        var groupsByCategories = AccountId.GetVAccountCategoryMonthlySums();

        if (groupsByCategories.Count is 0) return;

        var axis = groupsByCategories.First().Select(s => s.Period!);

        SetSeries(groupsByCategories);
        SetXAxis(axis);
        SetYAxis();
    }

    private void SetSeries(List<IGrouping<string?, GroupsByCategories>> groupsByCategories)
    {
        var series = groupsByCategories.GenerateSeries();
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