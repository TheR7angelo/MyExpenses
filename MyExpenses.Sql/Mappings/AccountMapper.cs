using Domain.Models.Accounts;
using Domain.Models.Categories;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Sql.Mappings;

[Mapper]
public static partial class AccountMapper
{
    public static partial IQueryable<TotalByAccountDomain> ProjectToDomain(this IQueryable<VTotalByAccount> src);

    public static partial IQueryable<AccountDomain> ProjectToDomain(this IQueryable<TAccount> src);

    public static partial IQueryable<CurrencyDomain> ProjectToDomain(this IQueryable<TCurrency> src);

    public static partial IQueryable<AccountTypeDomain> ProjectToDomain(this IQueryable<TAccountType> src);

    public static partial TotalByAccountDomain MapToDomain(this VTotalByAccount src);

    [MapProperty(nameof(TAccount.CurrencyFkNavigation), nameof(AccountDomain.CurrencyDomain))]
    [MapProperty(nameof(TAccount.AccountTypeFkNavigation), nameof(AccountDomain.AccountTypeDomain))]
    [MapperIgnoreSource(nameof(TAccount.AccountTypeFk))]
    [MapperIgnoreSource(nameof(TAccount.CurrencyFk))]
    [MapperIgnoreSource(nameof(TAccount.TBankTransferFromAccountFkNavigations))]
    [MapperIgnoreSource(nameof(TAccount.TBankTransferToAccountFkNavigations))]
    [MapperIgnoreSource(nameof(TAccount.THistories))]
    [MapperIgnoreSource(nameof(TAccount.TRecursiveExpenses))]
    public static partial AccountDomain MapToDomain(this TAccount src);

    [MapperIgnoreSource(nameof(TCurrency.TAccounts))]
    public static partial CurrencyDomain MapToDomain(this TCurrency src);

    [MapperIgnoreSource(nameof(TAccountType.TAccounts))]
    public static partial AccountTypeDomain MapToDomain(this TAccountType src);

    [MapperIgnoreTarget(nameof(TAccountType.TAccounts))]
    public static partial TAccountType MapToEntity(this AccountTypeDomain accountTypeDomain);

    [MapProperty(nameof(CategoryTypeDomain.Color.Id), nameof(TCategoryType.ColorFk))]
    [MapperIgnoreTarget(nameof(TCategoryType.ColorFkNavigation))]
    [MapperIgnoreTarget(nameof(TCategoryType.TRecursiveExpenses))]
    [MapperIgnoreTarget(nameof(TCategoryType.THistories))]
    public static partial TCategoryType MapToEntity(this CategoryTypeDomain categoryTypeDomain);
}