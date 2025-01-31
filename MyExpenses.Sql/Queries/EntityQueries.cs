using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Derivatives.Views;
using MyExpenses.Models.Sql.Queries;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.Sql.Context;

namespace MyExpenses.Sql.Queries;

public static class EntityQueries
{
    /// <summary>
    /// Applies the specified sort order to the given query.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements within the query.
    /// </typeparam>
    /// <param name="sortOrder">
    /// The sorting order to be applied. It can be <see cref="SortOrder.Ascending"/>, <see cref="SortOrder.Descending"/>, or <see cref="SortOrder.None"/>.
    /// </param>
    /// <param name="query">
    /// The query to which the sorting is to be applied.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> representing the query with the applied sorting order.
    /// If the sort order is <see cref="SortOrder.None"/>, the query is returned unmodified.
    /// </returns>
    private static IQueryable<T> ApplySortingToQuery<T>(this IQueryable<T> query, SortOrder sortOrder)
    {
        query = sortOrder switch
        {
            SortOrder.Ascending => query.OrderBy(year => year),
            SortOrder.Descending => query.OrderByDescending(year => year),
            _ => query
        };
        return query;
    }

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
    /// Retrieves the distinct years from the bank transfer records that have a valid date,
    /// optionally sorted by the specified sort order.
    /// </summary>
    /// <param name="context">
    /// The database context containing the bank transfer records.
    /// </param>
    /// <param name="sortOrder">
    /// The sort order to apply to the list of years. The default is <see cref="SortOrder.None"/>.
    /// </param>
    /// <returns>
    /// A collection of unique years extracted from the bank transfer dates.
    /// </returns>
    public static IEnumerable<int> GetDistinctYearsFromBankTransfer(this DataBaseContext context,
        SortOrder sortOrder = SortOrder.None)
    {
        var query = context.TBankTransfers
            .Where(s => s.Date.HasValue)
            .Select(s => s.Date!.Value.Year)
            .Distinct();

        query = query.ApplySortingToQuery(sortOrder);

        return query.AsEnumerable();
    }

    /// <summary>
    /// Retrieves a collection of distinct years from the history entries in the database,
    /// optionally sorted in the specified order.
    /// </summary>
    /// <param name="context">
    /// The database context containing the history data to query.
    /// </param>
    /// <param name="sortOrder">
    /// The order in which the years should be sorted. Can be None, Ascending, or Descending.
    /// Defaults to None.
    /// </param>
    /// <returns>
    /// An enumerable collection of distinct years retrieved from the history entries
    /// in the database.
    /// </returns>
    public static IEnumerable<int> GetDistinctYearsFromHistories(this DataBaseContext context,
        SortOrder sortOrder = SortOrder.None)
    {
        var query = context.THistories
            .Where(s => s.Date.HasValue)
            .Select(s => s.Date!.Value.Year)
            .Distinct();

        query = query.ApplySortingToQuery(sortOrder);

        return query.AsEnumerable();
    }

    /// <summary>
    /// Retrieves filtered bank transfers from the database based on the specified criteria.
    /// </summary>
    /// <param name="context">
    /// The database context to query the bank transfer summaries from.
    /// </param>
    /// <param name="year">
    /// The year to filter the bank transfers. If null, transfers from all years are included.
    /// </param>
    /// <param name="month">
    /// The month to filter the bank transfers. If null, transfers from all months are included.
    /// </param>
    /// <param name="fromAccounts">
    /// An array of account identifiers to filter transfers originating from specific accounts.
    /// If null or empty, no filtering is applied based on the source accounts.
    /// </param>
    /// <param name="toAccounts">
    /// An array of account identifiers to filter transfers targeting specific accounts.
    /// If null or empty, no filtering is applied based on the destination accounts.
    /// </param>
    /// <param name="values">
    /// An array of transaction values to filter the bank transfers. If null or empty,
    /// no filtering is applied based on the transaction values.
    /// </param>
    /// <param name="mainReasons">
    /// An array of main reasons to filter the bank transfers. If null or empty,
    /// no filtering is applied based on the main reasons.
    /// </param>
    /// <param name="additionalReasons">
    /// An array of additional reasons to filter the bank transfers. If null or empty,
    /// no filtering is applied based on the additional reasons.
    /// </param>
    /// <param name="categories">
    /// An array of categories to filter the bank transfers. If null or empty,
    /// no filtering is applied based on the categories.
    /// </param>
    /// <returns>
    /// A <see cref="FilteredBankTransfersResults"/> object containing the filtered bank transfers,
    /// the total row counts before filtering, and the row counts after applying filters.
    /// </returns>
    public static FilteredBankTransfersResults GetFilteredBankTransfers(this DataBaseContext context,
        int? year = null, int? month = null,
        string[]? fromAccounts = null, string[]? toAccounts = null,
        double[]? values = null, string[]? mainReasons = null, string?[]? additionalReasons = null,
        string[]? categories = null)
    {
        IQueryable<VBankTransferSummary> query = context.VBankTransferSummaries;
        if (!query.Any()) return new FilteredBankTransfersResults();

        if (year.HasValue)
        {
            query = query.Where(s => s.Date!.Value.Year.Equals(year.Value));
        }

        if (month.HasValue)
        {
            query = query.Where(s => s.Date!.Value.Month.Equals(month.Value));
        }

        var totalRowCount = query.Count();

        if (fromAccounts is { Length: > 0 })
        {
            query = query.Where(s => fromAccounts.Contains(s.FromAccountName));
        }

        if (toAccounts is { Length: > 0})
        {
            query = query.Where(s => toAccounts.Contains(s.ToAccountName));
        }

        if (values is { Length: > 0 })
        {
            query = query.Where(s => values.Contains(s.Value!.Value));
        }

        if (mainReasons is { Length: > 0 })
        {
            query = query.Where(s => mainReasons.Contains(s.MainReason));
        }

        if (additionalReasons is { Length: > 0})
        {
            query = query.Where(s => additionalReasons.Contains(s.AdditionalReason));
        }

        if (categories is { Length: > 0})
        {
            query = query.Where(s => categories.Contains(s.CategoryName));
        }

        var totalFilteredRowCount = query.Count();
        var records = query
            .OrderByDescending(s => s.Date).AsEnumerable();

        return new FilteredBankTransfersResults
        {
            BankTransferSummaries = records,
            TotalRowCount = totalRowCount,
            TotalFilteredRowCount = totalFilteredRowCount
        };
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
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Accessing the underlying data is only possible through the "DataBaseContext" object, which serves as the entry point
        // to the database. This allocation is mandatory to perform any query or operation on the "VHistories" view, and
        // without it, retrieving or filtering data would not be possible.
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
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Accessing the underlying data is only possible through the "DataBaseContext" object, which serves as the entry point
        // to the database. This allocation is mandatory to perform any query or operation on the "VDetailTotalCategories" view, and
        // without it, retrieving or filtering data would not be possible.
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

    public static IEnumerable<VRecursiveExpenseDerive> GetVRecursiveExpenseDerive()
    {
        var mapper = Mapping.Mapper;

        var now = DateTime.Now;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Accessing the underlying data is only possible through the "DataBaseContext" object, which serves as the entry point
        // to the database.
        using var context = new DataBaseContext();
        var records = context.TRecursiveExpenses
            .Where(s => !s.ForceDeactivate)
            .Where(s => s.IsActive)
            .Where(s => s.NextDueDate.Year.Equals(now.Year) && s.NextDueDate.Month.Equals(now.Month))
            .OrderBy(s => s.NextDueDate)
            .Select(s => s.Id.ToISql<VRecursiveExpense>())
            .Select(s => mapper.Map<VRecursiveExpenseDerive>(s))
            .ToList();

        return records;
    }

    /// <summary>
    /// Calculates category totals from a collection of detailed category data and computes the overall grand total.
    /// </summary>
    /// <param name="data">
    /// A collection of <see cref="VDetailTotalCategory"/> objects containing detailed information about categories.
    /// </param>
    /// <param name="grandTotal">
    /// An output parameter that will hold the computed grand total, which is the sum of absolute values of all category totals.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="CategoryTotalData"/> representing the totals for each category, ordered in descending order by the absolute value of the totals.
    /// </returns>
    public static IEnumerable<CategoryTotalData> CalculateCategoryTotals(this IEnumerable<VDetailTotalCategory> data,
        out double grandTotal)
    {
        var categoriesTotals = data
            .GroupBy(s => s.Category)
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The allocation here is necessary as a new instance of VDetailTotalCategory is created for each group.
            .Select(g => new CategoryTotalData
            {
                Category = g.Key,
                Total = Math.Round(g.Sum(s => s.Value) ?? 0d, 2),
                Symbol = g.First().Symbol,
                HexadecimalColorCode = g.First().HexadecimalColorCode
            })
            .OrderByDescending(s => Math.Abs(s.Total))
            .ToList();

        grandTotal = Math.Round(categoriesTotals.Sum(ct => Math.Abs(ct.Total)), 2);

        return categoriesTotals;
    }
}