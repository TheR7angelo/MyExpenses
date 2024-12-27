namespace MyExpenses.Models.Maui.CustomPopup;

public class BoolIsChecked : IEquatable<BoolIsChecked>
{
    public bool BoolValue { get; init; }
    public bool IsChecked { get; set; }

    public bool Equals(BoolIsChecked? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return BoolValue == other.BoolValue && IsChecked == other.IsChecked;
    }

    public override bool Equals(object? obj)
    {
        if (obj is BoolIsChecked other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BoolValue, IsChecked);
    }
}