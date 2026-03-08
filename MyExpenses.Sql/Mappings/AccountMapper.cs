using Domain.Models.Accounts;
using Riok.Mapperly.Abstractions;
using VTotalByAccount = MyExpenses.Sql.Entities.VTotalByAccount;

namespace MyExpenses.Sql.Mappings;

[Mapper]
public static partial class AccountMapper
{
    public static partial IQueryable<TotalByAccountDomain> ProjectToDto(this IQueryable<VTotalByAccount> src);

    public static partial TotalByAccountDomain MapToDomain(VTotalByAccount src);
}