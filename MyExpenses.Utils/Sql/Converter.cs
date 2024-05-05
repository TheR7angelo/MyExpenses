using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Utils.Sql;

public static class Converter
{
    public static VTotalByAccount? ToVTotalByAccount(this TAccount account)
    {
        using var context = new DataBaseContext();
        var vTotalByAccount = context.VTotalByAccounts.FirstOrDefault(s => s.Id == account.Id);

        return vTotalByAccount;
    }

    public static TAccount? ToTAccount(this VTotalByAccount vTotalByAccount)
    {
        using var context = new DataBaseContext();
        var account = context.TAccounts.FirstOrDefault(s => s.Id == vTotalByAccount.Id);

        return account;
    }
}