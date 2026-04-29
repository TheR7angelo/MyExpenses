using Domain.Models.Expenses;
using MyExpenses.Models.Sql.Bases.Tables;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Sql.Mappings;

[Mapper]
[UseStaticMapper(typeof(SystemMapper))]
[UseStaticMapper(typeof(AccountMapper))]
public static partial class ExpensesMapper
{
    public static partial IQueryable<CategoryTypeDomain> ProjectToDomain(this IQueryable<TCategoryType> src);

    public static partial IQueryable<HistoryDomain> ProjectToDomain(this IQueryable<THistory> src);

    public static partial IQueryable<ModePaymentDomain> ProjectToDomain(this IQueryable<TModePayment> src);

    public static partial IQueryable<TBankTransfer> ProjectToDomain(this IQueryable<TBankTransfer> src);

    public static partial IQueryable<RecursiveExpenseDomain> ProjectToDomain(this IQueryable<TRecursiveExpense> src);

    [MapperIgnoreTarget(nameof(THistory.AccountFkNavigation))]
    [MapperIgnoreTarget(nameof(THistory.BankTransferFkNavigation))]
    [MapperIgnoreTarget(nameof(THistory.BankTransferFkNavigation))]
    [MapperIgnoreTarget(nameof(THistory.CategoryTypeFkNavigation))]
    [MapperIgnoreTarget(nameof(THistory.ModePaymentFkNavigation))]
    [MapperIgnoreTarget(nameof(THistory.PlaceFkNavigation))]
    [MapperIgnoreTarget(nameof(THistory.RecursiveExpenseFkNavigation))]
    [MapProperty(nameof(HistoryDomain.Account.Id), nameof(THistory.AccountFk))]
    [MapProperty(nameof(HistoryDomain.BankTransfer.Id), nameof(THistory.BankTransferFk))]
    [MapProperty(nameof(HistoryDomain.RecursiveExpense.Id), nameof(THistory.RecursiveExpenseFk))]
    [MapProperty(nameof(HistoryDomain.CategoryType.Id), nameof(THistory.CategoryTypeFk))]
    [MapProperty(nameof(HistoryDomain.Place.Id), nameof(THistory.PlaceFk))]
    [MapProperty(nameof(HistoryDomain.ModePayment.Id), nameof(THistory.ModePaymentFk))]
    public static partial THistory MapToEntity(this HistoryDomain historyDomain);

    [MapperIgnoreSource(nameof(THistory.AccountFk))]
    [MapperIgnoreSource(nameof(THistory.CategoryTypeFk))]
    [MapperIgnoreSource(nameof(THistory.ModePaymentFk))]
    [MapperIgnoreSource(nameof(THistory.PlaceFk))]
    [MapperIgnoreSource(nameof(THistory.BankTransferFk))]
    [MapperIgnoreSource(nameof(THistory.RecursiveExpenseFk))]
    [MapProperty(nameof(THistory.CategoryTypeFkNavigation), nameof(HistoryDomain.CategoryType))]
    [MapProperty(nameof(THistory.AccountFkNavigation), nameof(HistoryDomain.Account))]
    [MapProperty(nameof(THistory.ModePaymentFkNavigation), nameof(HistoryDomain.ModePayment))]
    [MapProperty(nameof(THistory.PlaceFkNavigation), nameof(HistoryDomain.Place))]
    [MapProperty(nameof(THistory.BankTransferFkNavigation), nameof(HistoryDomain.BankTransfer))]
    [MapProperty(nameof(THistory.RecursiveExpenseFkNavigation), nameof(HistoryDomain.RecursiveExpense))]
    public static partial HistoryDomain MapToDomain(this THistory src);

    [MapProperty(nameof(CategoryTypeDomain.Color.Id), nameof(TCategoryType.ColorFk))]
    [MapperIgnoreTarget(nameof(TCategoryType.ColorFkNavigation))]
    [MapperIgnoreTarget(nameof(TCategoryType.TRecursiveExpenses))]
    [MapperIgnoreTarget(nameof(TCategoryType.THistories))]
    public static partial TCategoryType MapToEntity(this CategoryTypeDomain categoryTypeDomain);

    [MapProperty(nameof(TCategoryType.ColorFkNavigation), nameof(CategoryTypeDomain.Color))]
    [MapperIgnoreSource(nameof(TCategoryType.ColorFk))]
    [MapperIgnoreSource(nameof(TCategoryType.THistories))]
    [MapperIgnoreSource(nameof(TCategoryType.TRecursiveExpenses))]
    public static partial CategoryTypeDomain MapToDomain(this TCategoryType src);

    [MapperIgnoreSource(nameof(TModePayment.EModePayment))]
    [MapperIgnoreSource(nameof(TModePayment.THistories))]
    [MapperIgnoreSource(nameof(TModePayment.TRecursiveExpenses))]
    public static partial ModePaymentDomain MapToDomain(this TModePayment src);

    [MapperIgnoreTarget(nameof(TBankTransfer.THistories))]
    [MapperIgnoreTarget(nameof(TBankTransfer.FromAccountFkNavigation))]
    [MapperIgnoreTarget(nameof(TBankTransfer.ToAccountFkNavigation))]
    [MapProperty(nameof(BankTransferDomain.FromAccount.Id), nameof(TBankTransfer.FromAccountFk))]
    [MapProperty(nameof(BankTransferDomain.ToAccount.Id), nameof(TBankTransfer.ToAccountFk))]
    public static partial TBankTransfer MapToEntity(this BankTransferDomain bankTransferDomain);

    [MapperIgnoreSource(nameof(TBankTransfer.FromAccountFk))]
    [MapperIgnoreSource(nameof(TBankTransfer.ToAccountFk))]
    [MapperIgnoreSource(nameof(TBankTransfer.THistories))]
    [MapProperty(nameof(TBankTransfer.FromAccountFkNavigation), nameof(BankTransferDomain.FromAccount))]
    [MapProperty(nameof(TBankTransfer.ToAccountFkNavigation), nameof(BankTransferDomain.ToAccount))]
    public static partial BankTransferDomain MapToDomain(this TBankTransfer src);

    [MapperIgnoreTarget(nameof(TRecursiveExpense.THistories))]
    [MapperIgnoreTarget(nameof(TRecursiveExpense.PlaceFkNavigation))]
    [MapperIgnoreTarget(nameof(TRecursiveExpense.FrequencyFkNavigation))]
    [MapperIgnoreTarget(nameof(TRecursiveExpense.ERecursiveFrequency))]
    [MapperIgnoreTarget(nameof(TRecursiveExpense.CategoryTypeFkNavigation))]
    [MapperIgnoreTarget(nameof(TRecursiveExpense.ModePaymentFkNavigation))]
    [MapperIgnoreTarget(nameof(TRecursiveExpense.AccountFkNavigation))]
    [MapProperty(nameof(RecursiveExpenseDomain.Place.Id), nameof(TRecursiveExpense.PlaceFk))]
    [MapProperty(nameof(RecursiveExpenseDomain.RecursiveFrequency.Id), nameof(TRecursiveExpense.FrequencyFk))]
    [MapProperty(nameof(RecursiveExpenseDomain.ModePayment.Id), nameof(TRecursiveExpense.ModePaymentFk))]
    [MapProperty(nameof(RecursiveExpenseDomain.CategoryType.Id), nameof(TRecursiveExpense.CategoryTypeFk))]
    [MapProperty(nameof(RecursiveExpenseDomain.Account.Id), nameof(TRecursiveExpense.AccountFk))]
    public static partial TRecursiveExpense MapToEntity(this RecursiveExpenseDomain recursiveExpenseDomain);

    [MapperIgnoreSource(nameof(TRecursiveExpense.PlaceFk))]
    [MapperIgnoreSource(nameof(TRecursiveExpense.FrequencyFk))]
    [MapperIgnoreSource(nameof(TRecursiveExpense.ModePaymentFk))]
    [MapperIgnoreSource(nameof(TRecursiveExpense.CategoryTypeFk))]
    [MapperIgnoreSource(nameof(TRecursiveExpense.AccountFk))]
    [MapperIgnoreSource(nameof(TRecursiveExpense.THistories))]
    [MapperIgnoreSource(nameof(TRecursiveExpense.ERecursiveFrequency))]
    [MapperIgnoreSource(nameof(TRecursiveExpense.EModePaymentFk))]
    [MapProperty(nameof(TRecursiveExpense.PlaceFkNavigation), nameof(RecursiveExpenseDomain.Place))]
    [MapProperty(nameof(TRecursiveExpense.FrequencyFkNavigation), nameof(RecursiveExpenseDomain.RecursiveFrequency))]
    [MapProperty(nameof(TRecursiveExpense.ModePaymentFkNavigation), nameof(RecursiveExpenseDomain.ModePayment))]
    [MapProperty(nameof(TRecursiveExpense.CategoryTypeFkNavigation), nameof(RecursiveExpenseDomain.CategoryType))]
    [MapProperty(nameof(TRecursiveExpense.AccountFkNavigation), nameof(RecursiveExpenseDomain.Account))]
    public static partial RecursiveExpenseDomain MapToDomain(this TRecursiveExpense src);
}