namespace MyExpenses.SharedUtils.Maths;

public static class TrendUtils
{
    public static IEnumerable<double> CreateLinearTrendLine(this IEnumerable<double> values)
    {
        var enumerable = values as double[] ?? values.ToArray();

        var xData = Enumerable.Range(1, enumerable.Length).Select(i => (double)i).ToArray();
        var (a, b) = xData.CalculateLinearTrend(enumerable.ToArray());
        var trendValues = xData.Select(x => Math.Round(a * x + b, 2));

        return trendValues;
    }

    private static (double a, double b) CalculateLinearTrend(this double[] xData, double[] yData)
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
        var b = sumY / n - a * sumX / n; // intercept

        return (a, b);
    }
}