using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages;

public partial class DetailedRecordContentPage
{
    public static readonly BindableProperty HistoryProperty = BindableProperty.Create(nameof(History),
        typeof(THistory), typeof(DetailedRecordContentPage), default(THistory));

    public THistory? History
    {
        get => (THistory)GetValue(HistoryProperty);
        set => SetValue(HistoryProperty, value);
    }

    public DetailedRecordContentPage(int historyPk)
    {
        using var context = new DataBaseContext();
        History = context.THistories.FirstOrDefault(s => s.Id == historyPk);

        InitializeContentPage();
    }

    public DetailedRecordContentPage(THistory tHistory)
    {
        History = tHistory;

        InitializeContentPage();
    }

    private void InitializeContentPage()
    {
        InitializeComponent();
    }
}