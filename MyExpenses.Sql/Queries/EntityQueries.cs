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
    /// Retrieves a collection of filtered financial history records based on the provided account name,
    /// and optionally filtered by month and year.
    /// </summary>
    /// <param name="accountName">
    /// The name of the account for which the financial history is to be retrieved.
    /// </param>
    /// <param name="month">
    /// The optional month value to filter the history records. If null, no month-based filtering is applied.
    /// </param>
    /// <param name="year">
    /// The optional year value to filter the history records. If null, no year-based filtering is applied.
    /// </param>
    /// <returns>
    /// A collection of <see cref="VHistory"/> objects representing the filtered financial history records
    /// for the specified account name and optional time filters.
    /// </returns>
    public static FilteredHistoriesResults GetFilteredHistories(this string accountName,
        int? month = null, int? year = null,
        string[]? categories = null, string?[]? descriptions = null, string[]? modePayments = null, string[]? places = null,
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