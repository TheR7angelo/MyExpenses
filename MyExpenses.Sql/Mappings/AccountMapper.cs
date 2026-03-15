using Domain.Models.Accounts;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Sql.Mappings;

[Mapper]
public static partial class AccountMapper
{
    public static partial IQueryable<TotalByAccountDomain> ProjectToDomain(this IQueryable<VTotalByAccount> src);

    public static partial TotalByAccountDomain MapToDomain(VTotalByAccount src);

    public static partial IQueryable<AccountDomain> ProjectToDomain(this IQueryable<TAccount> src);

    [MapProperty(nameof(TAccount.CurrencyFkNavigation), nameof(AccountDomain.Currency))]
    [MapProperty(nameof(TAccount.AccountTypeFkNavigation), nameof(AccountDomain.AccountType))]
    [MapperIgnoreSource(nameof(TAccount.AccountTypeFk))]
    [MapperIgnoreSource(nameof(TAccount.CurrencyFk))]
    [MapperIgnoreSource(nameof(TAccount.TBankTransferFromAccountFkNavigations))]
    [MapperIgnoreSource(nameof(TAccount.TBankTransferToAccountFkNavigations))]
    [MapperIgnoreSource(nameof(TAccount.THistories))]
    [MapperIgnoreSource(nameof(TAccount.TRecursiveExpenses))]
    public static partial AccountDomain MapToDomain(TAccount src);

    public static partial IQueryable<CurrencyDomain> ProjectToDomain(this IQueryable<TCurrency> src);

    [MapperIgnoreSource(nameof(TCurrency.TAccounts))]
    public static partial CurrencyDomain MapToDomain(TCurrency src);

    public static partial IQueryable<AccountTypeDomain> ProjectToDomain(this IQueryable<TAccountType> src);

    [MapperIgnoreSource(nameof(TAccountType.TAccounts))]
    public static partial AccountTypeDomain MapToDomain(TAccountType src);
}