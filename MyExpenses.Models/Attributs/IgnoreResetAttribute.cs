namespace MyExpenses.Models.Attributs;

/// <summary>
/// An attribute that indicates a property has a default value.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IgnoreResetAttribute : Attribute
{

}