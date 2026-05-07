namespace MyExpenses.Presentation.Resources.Resx.AnalyticsResources;

public class AnalyticsResourceManager
{
    private static string RessourceManagerName => nameof(AnalyticsResources);

    public static string TabItemCumulativeSumChartControlHeader { get; } = $"{RessourceManagerName}:{nameof(AnalyticsResources.TabItemCumulativeSumChartControlHeader)}";
    public static string TabItemAccountTotalEllipseControlHeader { get; } = $"{RessourceManagerName}:{nameof(AnalyticsResources.TabItemAccountTotalEllipseControlHeader)}";
    public static string TabItemAccountsModePaymentMonthlySumControlHeader { get; } = $"{RessourceManagerName}:{nameof(AnalyticsResources.TabItemAccountsModePaymentMonthlySumControlHeader)}";
    public static string TabItemAccountsCategorySumControlHeader { get; } = $"{RessourceManagerName}:{nameof(AnalyticsResources.TabItemAccountsCategorySumControlHeader)}";
    public static string TabItemAccountsCategorySumPositiveNegativeControl { get; } = $"{RessourceManagerName}:{nameof(AnalyticsResources.TabItemAccountsCategorySumPositiveNegativeControl)}";
    public static string TabItemBudgetsControlHeader { get; } = $"{RessourceManagerName}:{nameof(AnalyticsResources.TabItemBudgetsControlHeader)}";
}