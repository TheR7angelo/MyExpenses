namespace MyExpenses.Utils;

public static class AnalyticsUtils
{
    public static (double a, double b) CalculateLinearTrend(double[] xData, double[] yData)
    {
        double sumX = 0, sumY = 0, sumXy = 0, sumXx = 0;

        var n = xData.Length;
        for (var i = 0; i < n; i++)
        {
            sumX += xData[i];
            sumY += yData[i];
            sumXy += xData[i] * yData[i];
            sumXx += xData[i] * xData[i];
        }

        var a = (n * sumXy - sumX * sumY) / (n * sumXx - sumX * sumX); // slope
        var b = (sumY / n) - (a * sumX / n); // intercept

        return (a, b);
    }
}