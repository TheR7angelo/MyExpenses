using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;

namespace MyExpenses.Smartphones.ContentPages;

public partial class DetailedRecordContentPage
{
    public static readonly BindableProperty HistoryProperty = BindableProperty.Create(nameof(History),
        typeof(THistory), typeof(DetailedRecordContentPage), default(THistory));

    public THistory History
    {
        get => (THistory)GetValue(HistoryProperty);
        set => SetValue(HistoryProperty, value);
    }

    public string CurrencySymbol { get; private set; } = null!;

    public ObservableCollection<TModePayment> ModePayments { get; private set; } = [];

    public DetailedRecordContentPage(int historyPk)
    {
        using var context = new DataBaseContext();
        History = context.THistories.First(s => s.Id.Equals(historyPk));

        InitializeContentPage();
    }

    public DetailedRecordContentPage(THistory tHistory)
    {
        History = tHistory;

        InitializeContentPage();
    }

    private void InitializeContentPage()
    {
        using var context = new DataBaseContext();
        ModePayments.AddRange(context.TModePayments);

        var account = context.TAccounts.First(s => s.Id.Equals(History.AccountFk));
        CurrencySymbol = context.TCurrencies.First(s => s.Id.Equals(account.CurrencyFk)).Symbol!;

        InitializeComponent();
    }
}