using Domain.Models.Dependencies;
using MyExpenses.Presentation.Resources.Resx.DependencyRessources;

namespace MyExpenses.Presentation.Converters;

public static class DependencyTypeConverter
{
    public static string? Convert(object? value, object? parameter)
    {
        if (value is not DependencyType dependencyType) return null;
        if (string.IsNullOrEmpty(parameter?.ToString())) return null;

        if (!bool.TryParse(parameter.ToString(), out var b)) return null;

        if (b)
        {
            return dependencyType switch
            {
                DependencyType.Account => DependencyRessources.DependencyTypeAccounts,
                DependencyType.BankTransfer => DependencyRessources.DependencyTypeBankTransfers,
                DependencyType.Expense => DependencyRessources.DependencyTypeExpenses,
                DependencyType.RecurringExpense => DependencyRessources.DependencyTypeRecurringExpenses,
                DependencyType.AccountType => DependencyRessources.DependencyTypeAccountTypes,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return dependencyType switch
        {
            DependencyType.Account => DependencyRessources.DependencyTypeAccount,
            DependencyType.BankTransfer => DependencyRessources.DependencyTypeBankTransfer,
            DependencyType.Expense => DependencyRessources.DependencyTypeExpense,
            DependencyType.RecurringExpense => DependencyRessources.DependencyTypeRecurringExpense,
            DependencyType.AccountType => DependencyRessources.DependencyTypeAccountType,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static DependencyType? ConvertBack(object? value, object? parameter)
    {
        if (value is not string str) return null;

        if (str.Equals(DependencyRessources.DependencyTypeAccount) ||
            str.Equals(DependencyRessources.DependencyTypeAccounts)) return DependencyType.Account;
        if (str.Equals(DependencyRessources.DependencyTypeBankTransfer) ||
            str.Equals(DependencyRessources.DependencyTypeBankTransfers)) return DependencyType.BankTransfer;
        if (str.Equals(DependencyRessources.DependencyTypeExpense) ||
            str.Equals(DependencyRessources.DependencyTypeExpenses)) return DependencyType.Expense;
        if (str.Equals(DependencyRessources.DependencyTypeRecurringExpense) ||
            str.Equals(DependencyRessources.DependencyTypeRecurringExpenses)) return DependencyType.RecurringExpense;
        if (str.Equals(DependencyRessources.DependencyTypeAccountType) ||
            str.Equals(DependencyRessources.DependencyTypeAccountTypes)) return DependencyType.AccountType;

        return null;
    }
}