namespace MyExpenses.Presentation.Resources.Resx.AnalyticsResources;

public class AnalyticsResourceManager
{
    private static string RessourceManagerName => nameof(AnalyticsResources);

    public static string TabItemCumulativeSumChartControlHeader => $"{RessourceManagerName}:{nameof(AnalyticsResources.TabItemCumulativeSumChartControlHeader)}";
    public static string TabItemAccountTotalEllipseControlHeader => $"{RessourceManagerName}:{nameof(AnalyticsResources.TabItemAccountTotalEllipseControlHeader)}";
    public static string TabItemAccountsModePaymentMonthlySumControlHeader => $"{RessourceManagerName}:{nameof(AnalyticsResources.TabItemAccountsModePaymentMonthlySumControlHeader)}";
    public static string TabItemAccountsCategorySumControlHeader => $"{RessourceManagerName}:{nameof(AnalyticsResources.TabItemAccountsCategorySumControlHeader)}";
    public static string TabItemAccountsCategorySumPositiveNegativeControl => $"{RessourceManagerName}:{nameof(AnalyticsResources.TabItemAccountsCategorySumPositiveNegativeControl)}";

    public static string TabItemBudgetsControlHeader =>
        $"{RessourceManagerName}:{nameof(AnalyticsResources.TabItemBudgetsControlHeader)}";
}