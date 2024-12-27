namespace MyExpenses.Models.Maui.CustomPopup;

public class StringIsChecked
{
    public string? StringValue { get; init; }
    public bool IsChecked { get; set; }

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
        return StringValue is null ? 0 : StringValue.GetHashCode();
    }
}