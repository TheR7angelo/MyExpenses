using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Regex;

namespace MyExpenses.Wpf.Pages;

public partial class RecordExpensePage
{

    public THistory History { get; } = new();

    public string SelectedValuePathAccount { get; } = nameof(TAccount.Id);
    public string DisplayMemberPathAccount { get; } = nameof(TAccount.Name);
    public string SelectedValuePathCategoryType { get; } = nameof(TCategoryType.Id);
    public string DisplayMemberPathCategoryType { get; } = nameof(TCategoryType.Name);
    public string SelectedValuePathModePayment { get; } = nameof(TModePayment.Id);
    public string DisplayMemberPathModePayment { get; } = nameof(TModePayment.Name);

    //TODO work
    public string ComboBoxAccountHintAssist { get; } = "From account :";
    //TODO work
    public string TextBoxDescriptionHintAssist { get; } = "Description :";
    //TODO work
    public string ComboBoxCategoryTypeHintAssist { get; } = "Category type :";
    //TODO work
    public string ComboBoxModePaymentHintAssist { get; } = "Mode payment :";
    //TODO work
    public string TextBoxValueHintAssist { get; } = "Value :";
    //TODO work
    public string DatePickerWhenHintAssist { get; } = "Date :";
    //TODO work
    public string TimePickerWhenHintAssist { get; } = "Time :";

    public required DashBoardPage DashBoardPage { get; set; }

    public ObservableCollection<TAccount> Accounts { get; }
    public ObservableCollection<TCategoryType> CategoryTypes { get; }
    public ObservableCollection<TModePayment> ModePayments { get; }

    public RecordExpensePage()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];
        CategoryTypes = [..context.TCategoryTypes.OrderBy(s => s.Name)];
        ModePayments = [..context.TModePayments.OrderBy(s => s.Name)];

        InitializeComponent();

        var language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        DatePicker.Language = language;

        var configuration = MyExpenses.Utils.Config.Configuration;
        TimePicker.Is24Hours = configuration.Interface.Clock.Is24Hours;
    }

    private void ButtonAccount_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
    }

    private void ButtonCategoryType_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
    }

    private void ButtonModePayment_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
    }

    private void TextBoxValue_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text;

        if (double.TryParse(txt, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            History.Value = value;
        else if (!txt.EndsWith('.')) History.Value = null;
    }

    private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text.Insert(textBox.SelectionStart, e.Text);

        e.Handled = txt.IsOnlyDecimal();
    }
}