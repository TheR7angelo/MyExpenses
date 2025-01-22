using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Queries;
using MyExpenses.Sql.Context;

namespace MyExpenses.Sql.Queries;

public static class EntityQueries
{
    /// <summary>
    /// Retrieves the active recurring expenses for the specified year and month
    /// from the database context.
    /// </summary>
    /// <param name="context">
    /// The database context to retrieve the recurring expenses from.
    /// </param>
    /// <param name="year">
    /// The year for which the recurring expenses are to be fetched.
    /// </param>
    /// <param name="month">
    /// The month for which the recurring expenses are to be fetched.
    /// This parameter must be between 1 and 12.
    /// </param>
    /// <returns>
    /// A collection of <see cref="TRecursiveExpense"/> objects for the specified
    /// year and month that are marked as active and not forcefully deactivated.
    /// </returns>
    public static IEnumerable<TRecursiveExpense> GetActiveRecurrencesForCurrentMonth(this DataBaseContext context,
        int year, int month)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(month, 12);

        return context.TRecursiveExpenses
            .Where(s => !s.ForceDeactivate)
            .Where(s => s.IsActive)
            .Where(s => s.NextDueDate.Year <= year && s.NextDueDate.Month <= month)
            .AsEnumerable();
    }

    /// <summary>
    /// Retrieves the distinct years from the "t_history" table where the date field has a value.
    /// </summary>
    /// <param name="context">
    /// The database context to query the "t_history" table for distinct years.
    /// </param>
    /// <returns>
    /// A collection of integer values representing distinct years ordered in descending order.
    /// </returns>
    public static IEnumerable<int> GetDistinctYearsFromHistories(this DataBaseContext context)
    {
        return context.THistories
            .Where(s => s.Date.HasValue)
            .Select(s => s.Date!.Value.Year)
            .Distinct()
            .OrderByDescending(year => year)
            .AsEnumerable();
    }

    /// <summary>
    /// Retrieves a distinct, ordered collection of years from bank transfer records
    /// where the date field has a valid value.
    /// </summary>
    /// <param name="context">
    /// The database context used to query the bank transfer records.
    /// </param>
    /// <returns>
    /// A collection of distinct integer values representing the years found in the
    /// bank transfer records, ordered in descending order.
    /// </returns>
    public static IEnumerable<int> GetDistinctYearsFromBankTransfer(this DataBaseContext context)
    {
        return context.TBankTransfers
            .Where(s => s.Date.HasValue)
            .Select(s => s.Date!.Value.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .AsEnumerable();
    }

    /// <summary>
    /// Retrieves a filtered list of transaction histories for the specified account
    /// based on various optional filters such as month, year, categories, descriptions,
    /// mode of payments, places, values, and pointed status.
    /// </summary>
    /// <param name="accountName">
    /// The name of the account for which the transaction histories should be retrieved.
    /// This parameter is required.
    /// </param>
    /// <param name="month">
    /// The specific month for filtering the transaction histories. This parameter is optional.
    /// </param>
    /// <param name="year">
    /// The specific year for filtering the transaction histories. This parameter is optional.
    /// </param>
    /// <param name="categories">
    /// An array of category names to filter the transaction histories. This parameter is optional.
    /// </param>
    /// <param name="descriptions">
    /// An array of descriptions to filter the transaction histories. This parameter is optional.
    /// </param>
    /// <param name="modePayments">
    /// An array of mode of payment methods to filter the transaction histories. This parameter is optional.
    /// </param>
    /// <param name="places">
    /// An array of place names to filter the transaction histories. This parameter is optional.
    /// </param>
    /// <param name="values">
    /// An array of values to filter the transaction histories. This parameter is optional.
    /// </param>
    /// <param name="pointed">
    /// An array of boolean values to filter the transaction histories based on whether they are pointed or not.
    /// This parameter is optional.
    /// </param>
    /// <returns>
    /// A <see cref="FilteredHistoriesResults"/> object containing the filtered transaction histories,
    /// along with the total row count and the total count of filtered rows.
    /// </returns>
    public static FilteredHistoriesResults GetFilteredHistories(this string accountName,
        int? month = null, int? year = null,
        string[]? categories = null, string?[]? descriptions = null, string[]? modePayments = null,
        string[]? places = null,
        double[]? values = null, bool[]? pointed = null)
    {
        using var context = new DataBaseContext();
        var query = context.VHistories
            .Where(s => s.Account == accountName);

        if (month.HasValue)
        {
            query = query.Where(s => s.Date!.Value.Month.Equals(month.Value));
        }

        if (year.HasValue)
        {
            query = query.Where(s => s.Date!.Value.Year.Equals(year.Value));
        }

        var totalRowCount = query.Count();

        if (categories is { Length: > 0 })
        {
            query = query.Where(s => categories.Contains(s.Category));
        }

        if (descriptions is { Length: > 0 })
        {
            query = query.Where(s => descriptions.Contains(s.Description));
        }

        if (modePayments is { Length: > 0 })
        {
            query = query.Where(s => modePayments.Contains(s.ModePayment));
        }

        if (places is { Length: > 0 })
        {
            query = query.Where(s => places.Contains(s.Place));
        }

        if (values is { Length: > 0 })
        {
            query = query.Where(s => values.Contains(s.Value!.Value));
        }

        if (pointed is { Length: > 0 })
        {
            query = query.Where(s => pointed.Contains((bool)s.IsPointed!));
        }

        var totalFilteredRowCount = query.Count();

        var records = query
            .OrderBy(s => s.IsPointed)
            .ThenByDescending(s => s.Date)
            .ThenBy(s => s.Category)
            .ToList();

        return new FilteredHistoriesResults { Histories = records, TotalRowCount = totalRowCount, TotalFilteredRowCount = totalFilteredRowCount};
    }

    /// <summary>
    /// Retrieves filtered total category details for a specific account name
    /// from the database context, optionally filtering by month and year.
    /// </summary>
    /// <param name="accountName">
    /// The name of the account for which the category details are to be fetched.
    /// </param>
    /// <param name="month">
    /// The optional month filter. If provided, results are filtered to only include
    /// data for the specified month (values should be 1 through 12).
    /// </param>
    /// <param name="year">
    /// The optional year filter. If provided, results are filtered to only include
    /// data for the specified year.
    /// </param>
    /// <returns>
    /// A collection of <see cref="VDetailTotalCategory"/> objects containing the
    /// filtered category total details based on the specified account name
    /// and optional filters for month and year.
    /// </returns>
    public static IEnumerable<VDetailTotalCategory> GetFilteredVDetailTotalCategories(this string accountName,
        int? month = null, int? year = null)
    {
        using var context = new DataBaseContext();

        var query = context.VDetailTotalCategories
            .Where(s => s.Account == accountName);

        if (month.HasValue)
        {
            query = query.Where(s => s.Month == month.Value);
        }

        if (year.HasValue)
        {
            query = query.Where(s => s.Year == year.Value);
        }

        return query.ToList();
    }
}