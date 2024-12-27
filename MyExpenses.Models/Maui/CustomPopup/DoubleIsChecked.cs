namespace MyExpenses.Models.Maui.CustomPopup;

public class DoubleIsChecked
{
    public double? DoubleValue { get; init; }
    public bool IsChecked { get; set; }

    private bool Equals(DoubleIsChecked? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return DoubleValue.Equals(other.DoubleValue) && IsChecked == other.IsChecked;
    }

    public override bool Equals(object? obj)
    {
        if (obj is DoubleIsChecked other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return  DoubleValue is null ? 0 : DoubleValue.GetHashCode();
    }
}