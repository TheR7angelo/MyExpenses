using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Core.UnitTests;

public static class TestDatabaseSeeder
{
    public static void Seed(DataBaseContext context)
    {
        var currencies = new List<TCurrency>
        {
            new() { Id = 1, Symbol = "USD", DateAdded = DateTime.Now },
            new() { Id = 2, Symbol = "EUR", DateAdded = DateTime.Now },
        };
        context.TCurrencies.AddRange(currencies);

        var accountTypes = new List<TAccountType>
        {
            new() { Id = 1, Name = "Savings", DateAdded = DateTime.Now },
            new() { Id = 2, Name = "Checking", DateAdded = DateTime.Now },
        };
        context.TAccountTypes.AddRange(accountTypes);

        var accounts = new List<TAccount>
        {
            new() { Id = 1, Name = "Primary Account", AccountTypeFk = 1, CurrencyFk = 1, Active = true, DateAdded = DateTime.Now },
            new() { Id = 2, Name = "Secondary Account", AccountTypeFk = 2, CurrencyFk = 2, Active = true, DateAdded = DateTime.Now },
        };
        context.TAccounts.AddRange(accounts);

        var colors = new List<TColor>
        {
            new() { Id = 1, Name = "Blue", HexadecimalColorCode = "#0000FF", DateAdded = DateTime.Now },
            new() { Id = 2, Name = "Red", HexadecimalColorCode = "#FF0000", DateAdded = DateTime.Now },
        };
        context.TColors.AddRange(colors);

        var categoryTypes = new List<TCategoryType>
        {
            new() { Id = 1, Name = "Food", ColorFk = 1, DateAdded = DateTime.Now },
            new() { Id = 2, Name = "Transport", ColorFk = 2, DateAdded = DateTime.Now },
        };
        context.TCategoryTypes.AddRange(categoryTypes);

        var places = new List<TPlace>
        {
            new() { Id = 1, Name = "Supermarket", City = "New York", Country = "USA", Latitude = 40.7128, Longitude = -74.0060, DateAdded = DateTime.Now },
            new() { Id = 2, Name = "Gas Station", City = "Los Angeles", Country = "USA", Latitude = 34.0522, Longitude = -118.2437, DateAdded = DateTime.Now },
        };
        context.TPlaces.AddRange(places);

        var modePayments = new List<TModePayment>
        {
            new() { Id = 1, Name = "Credit Card", CanBeDeleted = true, DateAdded = DateTime.Now },
            new() { Id = 2, Name = "Cash", CanBeDeleted = true, DateAdded = DateTime.Now },
        };
        context.TModePayments.AddRange(modePayments);

        var bankTransfers = new List<TBankTransfer>
        {
            new() { Id = 1, FromAccountFk = 1, ToAccountFk = 2, Value = 150.75, MainReason = "Transfer Savings", Date = DateTime.Now },
            new() { Id = 2, FromAccountFk = 2, ToAccountFk = 1, Value = 200.50, MainReason = "Refund", Date = DateTime.Now },
        };
        context.TBankTransfers.AddRange(bankTransfers);

        var histories = new List<THistory>
        {
            new()
            {
                Id = 1, AccountFk = 1, Description = "Groceries", CategoryTypeFk = 1, ModePaymentFk = 1, Value = 75.0, PlaceFk = 1, IsPointed = true,
                BankTransferFk = 1, Date = DateTime.Now
            },
            new()
            {
                Id = 2, AccountFk = 2, Description = "Fuel", CategoryTypeFk = 2, ModePaymentFk = 2, Value = 50.0, PlaceFk = 2, IsPointed = false,
                BankTransferFk = 2, Date = DateTime.Now
            },
        };
        context.THistories.AddRange(histories);

        var frequencies = new List<TRecursiveFrequency>
        {
            new() { Id = 1, Frequency = "Monthly", Description = "Every month" },
            new() { Id = 2, Frequency = "Yearly", Description = "Once a year" },
        };
        context.TRecursiveFrequencies.AddRange(frequencies);

        var recursiveExpenses = new List<TRecursiveExpense>
        {
            new()
            {
                Id = 1, AccountFk = 1, Description = "Subscription", CategoryTypeFk = 1, PlaceFk = 1, ModePaymentFk = 1,
                Value = 20.0, FrequencyFk = 1, StartDate = DateOnly.FromDateTime(DateTime.Now), NextDueDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)),
                RecursiveTotal = 12, RecursiveCount = 1, IsActive = true, ForceDeactivate = false,
            },
            new()
            {
                Id = 2, AccountFk = 2, Description = "Gym Membership", CategoryTypeFk = 2, PlaceFk = 2, ModePaymentFk = 2,
                Value = 50.0, FrequencyFk = 2, StartDate = DateOnly.FromDateTime(DateTime.Now), NextDueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
                RecursiveTotal = 1, RecursiveCount = 0, IsActive = true, ForceDeactivate = false,
            },
        };
        context.TRecursiveExpenses.AddRange(recursiveExpenses);

        context.SaveChanges();
    }
}