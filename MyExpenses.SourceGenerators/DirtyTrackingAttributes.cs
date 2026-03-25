namespace MyExpenses.SourceGenerators;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DirtyTrackingAttribute : Attribute
{
}

public enum DirtyComparisonKind
{
    Simple,
    NumericWithTolerance,
    Complex
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class DirtyTrackedPropertyAttribute : Attribute
{
    public DirtyTrackedPropertyAttribute(DirtyComparisonKind comparisonKind = DirtyComparisonKind.Simple)
    {
        ComparisonKind = comparisonKind;
    }

    public DirtyComparisonKind ComparisonKind { get; }

    public double Tolerance { get; set; } = 0.0001;
}