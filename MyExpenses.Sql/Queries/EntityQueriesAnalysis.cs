using MyExpenses.Models.Sql.Bases.Groups.VAccountCategoryMonthlySums;
using MyExpenses.Models.Sql.Bases.Groups.VAccountModePaymentCategoryMonthlySums;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Sql.Context;

namespace MyExpenses.Sql.Queries;

public static class EntityQueriesAnalysis
{
    /// <summary>
    /// Retrieves the monthly sums of payments for a given account, grouped by mode of payment, category, and period.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account to retrieve the data for.</param>
    /// <returns>An enumerable collection of groups where each group contains records grouped by mode of payment.</returns>
    public static List<IGrouping<string?, GroupsByModePaymentCategory>> GetVAccountModePaymentCategoryMonthlySums(this int accountId)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Retrieve records from the database filtered by AccountFk.
        // The use of `.AsEnumerable()` here is necessary to switch
        // the subsequent operations (like GroupBy) from being processed
        // by the LINQ to Entities query provider (EF Core) to LINQ to Objects,
        // as operations like GroupBy are not always supported or optimized
        // by the SQL database engine.
        using var context = new DataBaseContext();
        var groupsByModePaymentCategory = context.AnalysisVAccountModePaymentCategoryMonthlySums
            .Where(s => s.AccountFk == accountId)
            .GroupBy(v => new { v.AccountFk, v.Account, v.ModePayment, v.Period, v.CurrencyFk, v.Currency })
            .Select(g => new GroupsByModePaymentCategory
            {
                AccountFk = g.Key.AccountFk,
                Account = g.Key.Account,
                ModePayment = g.Key.ModePayment,
                Period = g.Key.Period,
                TotalMonthlySum = g.Sum(v => Math.Round(v.MonthlySum ?? 0, 2)),
                CurrencyFk = g.Key.CurrencyFk,
                Currency = g.Key.Currency,
                TotalMonthlyModePayment = g.Sum(v => v.MonthlyModePayment)
            })
            .OrderBy(s => s.Period).ThenBy(s => s.ModePayment)
            .AsEnumerable()
            .GroupBy(s => s.ModePayment)
            .ToList();

        return groupsByModePaymentCategory;
    }

    /// <summary>
    /// Retrieves the monthly positive and negative sum of a specified account category grouped by period.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account to retrieve the data for.</param>
    /// <returns>An enumerable collection of groups where each group contains records grouped by period.</returns>
    public static List<IGrouping<string?, AnalysisVAccountCategoryMonthlySumPositiveNegative>> GetVAccountCategoryMonthlySumPositiveNegative(this int accountId)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Retrieve records from the database filtered by AccountFk.
        // The use of `.AsEnumerable()` here is necessary to switch
        // the subsequent operations (like GroupBy) from being processed
        // by the LINQ to Entities query provider (EF Core) to LINQ to Objects,
        // as operations like GroupBy are not always supported or optimized
        // by the SQL database engine.
        using var context = new DataBaseContext();
        var records = context.AnalysisVAccountCategoryMonthlySumPositiveNegatives
            .Where(s => s.AccountFk == accountId)
            .AsEnumerable()
            .GroupBy(s => s.Period)
            .ToList();

        return records;
    }

    /// <summary>
    /// Retrieves the monthly sums of expenses for a given account, grouped by category and period.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account to retrieve data for.</param>
    /// <returns>A list of grouped results, where each group contains records classified by category type.</returns>
    public static List<IGrouping<string?, GroupsByCategories>> GetVAccountCategoryMonthlySums(this int accountId)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Retrieve records from the database filtered by AccountFk.
        using var context = new DataBaseContext();

        var groupsByCategories = context.AnalysisVAccountCategoryMonthlySums
            .Where(s => s.AccountFk == accountId)
            .GroupBy(s => new { s.AccountFk, s.Account, s.Period, s.CategoryType, s.ColorCode, s.CurrencyFk, s.Currency })
            .Select(g => new GroupsByCategories
            {
                AccountFk = g.Key.AccountFk,
                Account = g.Key.Account,
                Period = g.Key.Period,
                CategoryType = g.Key.CategoryType,
                ColorCode = g.Key.ColorCode,
                SumMonthlySum = Math.Round(g.Sum(v => v.MonthlySum ?? 0), 2),
                CurrencyFk = g.Key.CurrencyFk,
                Currency = g.Key.Currency
            })
            .OrderBy(s => s.Period).ThenBy(s => s.CategoryType)
            .AsEnumerable()
            .GroupBy(s => s.CategoryType)
            .ToList();
        return groupsByCategories;
    }
}