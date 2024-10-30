using System.Collections.ObjectModel;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.DetailedRecordContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;

namespace MyExpenses.Smartphones.ContentPages;

public partial class DetailedRecordContentPage
{
    public static readonly BindableProperty PointedOperationProperty = BindableProperty.Create(nameof(PointedOperation),
        typeof(string), typeof(DetailedRecordContentPage), default(string));

    public string PointedOperation
    {
        get => (string)GetValue(PointedOperationProperty);
        set => SetValue(PointedOperationProperty, value);
    }

    public static readonly BindableProperty LabelTextAddedOnProperty = BindableProperty.Create(nameof(LabelTextAddedOn),
        typeof(string), typeof(DetailedRecordContentPage), default(string));

    public string LabelTextAddedOn
    {
        get => (string)GetValue(LabelTextAddedOnProperty);
        set => SetValue(LabelTextAddedOnProperty, value);
    }

    public static readonly BindableProperty THistoryProperty = BindableProperty.Create(nameof(THistory),
        typeof(THistory), typeof(DetailedRecordContentPage), default(THistory));

    public THistory THistory
    {
        get => (THistory)GetValue(THistoryProperty);
        set => SetValue(THistoryProperty, value);
    }

    public static readonly BindableProperty VHistoryProperty = BindableProperty.Create(nameof(VHistory),
        typeof(VHistory), typeof(DetailedRecordContentPage), default(VHistory));

    public VHistory VHistory
    {
        get => (VHistory)GetValue(VHistoryProperty);
        set => SetValue(VHistoryProperty, value);
    }

    public ObservableCollection<TModePayment> ModePayments { get; private set; } = [];
    public ObservableCollection<TCategoryType> CategoryTypes { get; private set; } = [];

    public DetailedRecordContentPage(int historyPk)
    {
        using var context = new DataBaseContext();
        THistory = context.THistories.First(s => s.Id.Equals(historyPk));
        VHistory = context.VHistories.First(s => s.Id.Equals(historyPk));

        InitializeContentPage();
    }

    public DetailedRecordContentPage(THistory tHistory)
    {
        using var context = new DataBaseContext();
        THistory = tHistory;
        VHistory = context.VHistories.First(s => s.Id.Equals(tHistory.Id));

        InitializeContentPage();
    }

    public DetailedRecordContentPage(VHistory vHistory)
    {
        using var context = new DataBaseContext();
        THistory = context.THistories.First(s => s.Id.Equals(vHistory.Id));
        VHistory = vHistory;

        InitializeContentPage();
    }

    private void InitializeContentPage()
    {
        using var context = new DataBaseContext();
        ModePayments.AddRange(context.TModePayments);
        CategoryTypes.AddRange(context.TCategoryTypes);

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        LabelTextAddedOn = DetailedRecordContentPageResources.LabelTextAddedOn;
        PointedOperation = DetailedRecordContentPageResources.PointedOperation;
    }
}