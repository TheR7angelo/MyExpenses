namespace MyExpenses.Models.Maui.CustomPopup;

public class StringIsChecked
{
    public string? StringValue { get; }
    public bool IsChecked { get; }

    public bool Equals(StringIsChecked? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return StringValue == other.StringValue && IsChecked == other.IsChecked;
    }

    public override bool Equals(object? obj)
    {
        if (obj is StringIsChecked other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StringValue, IsChecked);
    }
}