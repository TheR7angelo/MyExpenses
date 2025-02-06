using System.Windows;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Groups.VAccountModePaymentCategoryMonthlySums;
using MyExpenses.Share.Core.Analysis;
using MyExpenses.SharedUtils.Converters;
using MyExpenses.Sql.Queries;
using MyExpenses.Utils;

namespace MyExpenses.Wpf.UserControls.Analytics.AccountsModePaymentMonthlySumControls;

public partial class AccountModePaymentMonthlySumControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(SolidColorPaint), typeof(AccountModePaymentMonthlySumControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    private int AccountId { get; }

    public AccountModePaymentMonthlySumControl(int accountId)
    {
        AccountId = accountId;

        UpdateTextPaint();
        SetChart();
        UpdateLanguage();

        InitializeComponent();

        // ReSharper disable HeapView.DelegateAllocation
        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
        // ReSharper restore HeapView.DelegateAllocation
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UpdateTextPaint()
    {
        var skColor = Utils.Resources.GetMaterialDesignBodySkColor();
        TextPaint = skColor.ToSolidColorPaint();
    }

    private void Interface_OnThemeChanged()
    {
        UpdateTextPaint();
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

        for (var i = 0; i < Series.Length; i++)
        {
            var tmp = Series[i] as ColumnSeries<double>;
            tmp!.DataLabelsPaint = TextPaint.Color.ToSolidColorPaint();
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

        UpdateLayout();
    }

    private void SetChart()
    {
        var groupsByModePaymentCategory = AccountId.GetVAccountModePaymentCategoryMonthlySums();

        if (groupsByModePaymentCategory.Count is 0) return;

        var axis = groupsByModePaymentCategory.First().Select(s => s.Period!);

        SetSeries(groupsByModePaymentCategory);
        SetXAxis(axis);
        SetYAxis();
    }

    private void SetYAxis()
    {
        var axis = TextPaint.CreateAxis();
        YAxis = [axis];
    }

    private void SetXAxis(IEnumerable<string> labels)
    {
        var transformedLabels = labels.ToTransformLabelsToTitleCaseDateFormat();
        var axis = transformedLabels.CreateAxis(TextPaint);
        XAxis = [axis];
    }

    private void SetSeries(List<IGrouping<string?, GroupsByModePaymentCategory>> groupsByModePayments)
    {
        var solidColorPaint = TextPaint.Color.ToSolidColorPaint();
        var series = groupsByModePayments.GenerateSeries(solidColorPaint);
        Series = [..series];
    }
}