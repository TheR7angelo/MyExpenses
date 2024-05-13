using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Utils.Sql;

public static class Converter
{
    public static TColor? ToTColor(this int id)
    {
        using var context = new DataBaseContext();
        var color = context.TColors.FirstOrDefault(s => s.Id == id);

        return color;
    }

    public static THistory? ToTHistory(this VHistory vHistory)
    {
        using var context = new DataBaseContext();
        var history = context.THistories.FirstOrDefault(s => s.Id == vHistory.Id);

        return history;
    }

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