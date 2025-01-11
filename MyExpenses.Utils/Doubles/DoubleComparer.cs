namespace MyExpenses.Utils.Doubles;

public static class DoubleComparer
{
    public static bool AreEqual(this double value1, double value2, double tolerance = 1e-9)
        => Math.Abs(value1 - value2) < tolerance;
}