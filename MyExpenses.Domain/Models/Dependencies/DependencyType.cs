namespace Domain.Models.Dependencies;

/// <summary>
/// Represents the various types of dependencies that can occur within the domain model.
/// </summary>
public enum DependencyType
{
    Account,
    BankTransfer,
    CategoryType,
    Expense,
    RecurringExpense,
    AccountType
}

public static class DependencyCaptionName
{
    /// <summary>
    /// Retrieves the resource icon name associated with the specified dependency type.
    /// </summary>
    /// <param name="dependencyType">The type of dependency for which the icon name is requested.</param>
    /// <returns>The resource icon name corresponding to the given dependency type.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the provided dependency type is not a valid <see cref="DependencyType"/> value.
    /// </exception>
    public static string GetRessourceIconName(this DependencyType dependencyType)
        => dependencyType switch
        {
            DependencyType.Account => "Account",
            DependencyType.BankTransfer => "AccountTransfert",
            DependencyType.Expense => "Ticket",
            DependencyType.RecurringExpense => "RecursiveExpense",
            _ => throw new ArgumentOutOfRangeException(nameof(dependencyType), dependencyType, null)
        };
}