using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AddEditAccountContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AddEditAccountContentPage
{
    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(AddEditAccountContentPage), default(string));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty CanDeleteProperty = BindableProperty.Create(nameof(CanDelete), typeof(bool),
        typeof(AddEditAccountContentPage), default(bool));

    public bool CanDelete
    {
        get => (bool)GetValue(CanDeleteProperty);
        set => SetValue(CanDeleteProperty, value);
    }

    private List<TAccount> Accounts { get; }
    public TAccount Account { get; } = new();

    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public AddEditAccountContentPage()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts];

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        PlaceholderText = AddEditAccountContentPageResources.PlaceholderText;
    }

    public void SetAccount(TAccount? account = null, int? id = null)
    {
        if (account is not null) account.CopyPropertiesTo(Account);
        else if (id is not null)
        {
            account = Accounts.First(s => s.Id.Equals(id));
            account.CopyPropertiesTo(Account);
        }
        else throw new ArgumentNullException(nameof(account), @"account is null");
    }
}