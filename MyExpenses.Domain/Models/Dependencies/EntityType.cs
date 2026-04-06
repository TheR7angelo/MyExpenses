namespace Domain.Models.Dependencies;

/// <summary>
/// Represents the various types of dependencies that can occur within the domain model.
/// </summary>
public enum EntityType
{
    Account,
    BankTransfer,
    Expense,
    RecurringExpense,
    AccountType
}

public static class DependencyCaptionName
{
    /// <summary>
    /// Retrieves the resource icon name associated with the specified dependency type.
    /// </summary>
    /// <param name="entityType">The type of dependency for which the icon name is requested.</param>
    /// <returns>The resource icon name corresponding to the given dependency type.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the provided dependency type is not a valid <see cref="EntityType"/> value.
    /// </exception>
    public static string GetRessourceIconName(this EntityType entityType)
        => entityType switch
        {
            EntityType.Account => "Account",
            EntityType.BankTransfer => "AccountTransfert",
            EntityType.Expense => "Ticket",
            EntityType.RecurringExpense => "RecursiveExpense",
            _ => throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null)
        };
}