using MyExpenses.Presentation.Resources.Resx.SystemResources;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation;

public sealed class BoolDisplayValueProvider : IDirtyTrackingDisplayValueProvider
{
    public string? GetDisplayValue(object? value)
    {
        return value switch
        {
            true => SystemResources.BoolDisplayValueProviderTrueValue,
            false => SystemResources.BoolDisplayValueProviderFalseValue,
            null => null,
            _ => value.ToString()
        };
    }
}