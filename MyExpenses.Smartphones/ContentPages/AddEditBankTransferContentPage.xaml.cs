using System.Collections.ObjectModel;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AddEditBankTransferContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Objects;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AddEditBankTransferContentPage
{
    public static readonly BindableProperty ButtonCancelUpdateTextProperty =
        BindableProperty.Create(nameof(ButtonCancelUpdateText), typeof(string), typeof(AddEditBankTransferContentPage),
            default(string));

    public string ButtonCancelUpdateText
    {
        get => (string)GetValue(ButtonCancelUpdateTextProperty);
        set => SetValue(ButtonCancelUpdateTextProperty, value);
    }

    public static readonly BindableProperty ButtonCanBeDeletedTextProperty =
        BindableProperty.Create(nameof(ButtonCanBeDeletedText), typeof(string), typeof(AddEditBankTransferContentPage),
            default(string));

    public string ButtonCanBeDeletedText
    {
        get => (string)GetValue(ButtonCanBeDeletedTextProperty);
        set => SetValue(ButtonCanBeDeletedTextProperty, value);
    }

    public static readonly BindableProperty ButtonUpdateTextProperty = BindableProperty.Create(nameof(ButtonUpdateText),
        typeof(string), typeof(AddEditBankTransferContentPage), default(string));

    public string ButtonUpdateText
    {
        get => (string)GetValue(ButtonUpdateTextProperty);
        set => SetValue(ButtonUpdateTextProperty, value);
    }

    public static readonly BindableProperty CanBeDeletedProperty = BindableProperty.Create(nameof(CanBeDeleted),
        typeof(bool), typeof(AddEditBankTransferContentPage), default(bool));

    public bool CanBeDeleted
    {
        get => (bool)GetValue(CanBeDeletedProperty);
        set => SetValue(CanBeDeletedProperty, value);
    }

    public static readonly BindableProperty IsDirtyProperty = BindableProperty.Create(nameof(IsDirty), typeof(bool),
        typeof(AddEditBankTransferContentPage), default(bool));

    public bool IsDirty
    {
        get => (bool)GetValue(IsDirtyProperty);
        set => SetValue(IsDirtyProperty, value);
    }

    public static readonly BindableProperty LabelTextTransferDateProperty =
        BindableProperty.Create(nameof(LabelTextTransferDate), typeof(string), typeof(AddEditBankTransferContentPage),
            default(string));

    public string LabelTextTransferDate
    {
        get => (string)GetValue(LabelTextTransferDateProperty);
        set => SetValue(LabelTextTransferDateProperty, value);
    }

    public static readonly BindableProperty LabelTextFromAccountFromProperty =
        BindableProperty.Create(nameof(LabelTextFromAccountFrom), typeof(string),
            typeof(AddEditBankTransferContentPage), default(string));

    public string LabelTextFromAccountFrom
    {
        get => (string)GetValue(LabelTextFromAccountFromProperty);
        set => SetValue(LabelTextFromAccountFromProperty, value);
    }

    public TBankTransfer BankTransfer { get; } = new();
    public TBankTransfer? OriginalBankTransfer { get; private set; }

    private List<TAccount> Accounts { get; }
    public ObservableCollection<TAccount> FromAccounts { get; } = [];
    public ObservableCollection<TAccount> ToAccounts { get; } = [];

    public AddEditBankTransferContentPage()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];
        FromAccounts.AddRange(Accounts);
        ToAccounts.AddRange(Accounts);

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        ButtonUpdateText = AddEditBankTransferContentPageResources.ButtonUpdateText;
        ButtonCanBeDeletedText = AddEditBankTransferContentPageResources.ButtonCanBeDeletedText;
        ButtonCancelUpdateText = AddEditBankTransferContentPageResources.ButtonCancelUpdateText;

        LabelTextFromAccountFrom = AddEditBankTransferContentPageResources.LabelTextFromAccountFrom;
        LabelTextTransferDate = AddEditBankTransferContentPageResources.LabelTextTransferDate;
    }

    public void SetVBankTransferSummary(VBankTransferSummary? vBankTransferSummary)
    {
        if (vBankTransferSummary is null) return;

        var bankTransfer = vBankTransferSummary.Id.ToISql<TBankTransfer>()!;
        bankTransfer.CopyPropertiesTo(BankTransfer);
        OriginalBankTransfer = bankTransfer.DeepCopy();
    }

    private void PickerFromAccount_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        // TODO work
        var accountId = BankTransfer.FromAccountFk!;
    }

    private void ButtonUpdateBankTransfer_OnClicked(object? sender, EventArgs e)
    {
        // TODO work
        throw new NotImplementedException();
    }

    private void ButtonDeleteBankTransfer_OnClicked(object? sender, EventArgs e)
    {
        // TODO work
        throw new NotImplementedException();
    }

    private void ButtonCancelUpdateBankTransfer_OnClicked(object? sender, EventArgs e)
    {
        // TODO work
        throw new NotImplementedException();
    }
}