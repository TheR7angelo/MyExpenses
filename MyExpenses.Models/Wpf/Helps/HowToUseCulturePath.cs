using System.Globalization;

namespace MyExpenses.Models.Wpf.Helps;

public struct HowToUseCulturePath
{
    public required CultureInfo CultureInfo { get; init; }
    public required string Path { get; init; }
}