using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Utils.Sql;

public static class Converter
{
    /// <summary>
    /// Converts an integer to a TColor object.
    /// </summary>
    /// <param name="id">The id of the TColor.</param>
    /// <returns>The TColor object with the specified id, or null if no TColor is found with the id.</returns>
    public static TColor? ToTColor(this int id)
    {
        using var context = new DataBaseContext();
        var color = context.TColors.FirstOrDefault(s => s.Id == id);

        return color;
    }

    /// <summary>
    /// Converts a VHistory object to a THistory object.
    /// </summary>
    /// <param name="vHistory">The VHistory object to convert.</param>
    /// <returns>The THistory object that corresponds to the VHistory object, or null if no THistory is found with the specified id.</returns>
    public static THistory? ToTHistory(this VHistory vHistory)
    {
        using var context = new DataBaseContext();
        var history = context.THistories.FirstOrDefault(s => s.Id == vHistory.Id);

        return history;
    }

    /// <summary>
    /// Converts a TAccount object to a VTotalByAccount object.
    /// </summary>
    /// <param name="account">The TAccount object to convert.</param>
    /// <returns>The VTotalByAccount object with the same Id as the TAccount object, or null if no VTotalByAccount is found with the Id.</returns>
    public static VTotalByAccount? ToVTotalByAccount(this TAccount account)
    {
        using var context = new DataBaseContext();
        var vTotalByAccount = context.VTotalByAccounts.FirstOrDefault(s => s.Id == account.Id);

        return vTotalByAccount;
    }

    /// <summary>
    /// Converts an integer representing an account id to a VTotalByAccount object.
    /// </summary>
    /// <param name="accountId">The id of the account.</param>
    /// <returns>The VTotalByAccount object with the specified id, or null if no VTotalByAccount is found with the id.</returns>
    public static VTotalByAccount? ToVTotalByAccount(this int accountId)
    {
        using var context = new DataBaseContext();
        var vTotalByAccount = context.VTotalByAccounts.FirstOrDefault(s => s.Id == accountId);

        return vTotalByAccount;
    }

    /// <summary>
    /// Converts a VTotalByAccount object to a TAccount object.
    /// </summary>
    /// <param name="vTotalByAccount">The VTotalByAccount object to convert.</param>
    /// <returns>The TAccount object with the specified id, or null if no TAccount is found with the id.</returns>
    public static TAccount? ToTAccount(this VTotalByAccount vTotalByAccount)
    {
        using var context = new DataBaseContext();
        var account = context.TAccounts.FirstOrDefault(s => s.Id == vTotalByAccount.Id);

        return account;
    }

    /// <summary>
    /// Converts an integer to a TAccount object.
    /// </summary>
    /// <param name="accountId">The id of the TAccount.</param>
    /// <returns>The TAccount object with the specified id, or null if no TAccount is found with the id.</returns>
    public static TAccount? ToTAccount(this int accountId)
    {
        using var context = new DataBaseContext();
        var account = context.TAccounts.FirstOrDefault(s => s.Id == accountId);

        return account;
    }
}