namespace MyExpenses.Models.Maui.CustomPopup;

public class DoubleIsChecked
{
    private readonly Guid _objectId = Guid.NewGuid();
    public double? DoubleValue { get; init; }
    public bool IsChecked { get; set; }

    private bool Equals(DoubleIsChecked? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return EqualityComparer<double?>.Default.Equals(DoubleValue, other.DoubleValue)
               && IsChecked == other.IsChecked;
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
       => _objectId.GetHashCode();
}