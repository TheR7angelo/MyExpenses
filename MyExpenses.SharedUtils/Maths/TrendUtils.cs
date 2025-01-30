namespace MyExpenses.SharedUtils.Maths;

public static class TrendUtils
{
    /// <summary>
    /// Generates a linear trend line based on the provided collection of numerical values.
    /// </summary>
    /// <param name="values">The sequence of numerical values for which the trend line is to be calculated.</param>
    /// <returns>An enumerable collection of calculated trend line values.</returns>
    public static IEnumerable<double> CreateLinearTrendLine(this IEnumerable<double> values)
    {
        var enumerable = values as double[] ?? values.ToArray();

        var xData = Enumerable.Range(1, enumerable.Length).Select(i => (double)i).ToArray();
        var (a, b) = xData.CalculateLinearTrend(enumerable.ToArray());
        var trendValues = xData.Select(x => Math.Round(a * x + b, 2));

        return trendValues;
    }

    /// <summary>
    /// Calculates the slope and intercept of a linear trend based on the provided x and y datasets.
    /// </summary>
    /// <param name="xData">The array of independent variable values (x-axis).</param>
    /// <param name="yData">The array of dependent variable values (y-axis).</param>
    /// <returns>A tuple containing the slope (a) and the intercept (b) of the linear trend.</returns>
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