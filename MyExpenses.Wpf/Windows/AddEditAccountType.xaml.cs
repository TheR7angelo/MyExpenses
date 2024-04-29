using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.AddEditAccountType;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditAccountType
{
    public TAccountType AccountType { get; } = new();

    public ObservableCollection<TAccountType> AccountTypes { get; }

    public string TextBoxAccountTypeName { get; } = AddEditAccountTypeResources.TextBoxAccountTypeName;
    public string ButtonValidContent { get; } = AddEditAccountTypeResources.ButtonValidContent;
    public string ButtonDeleteContent { get; } = AddEditAccountTypeResources.ButtonDeleteContent;
    public string ButtonCancelContent { get; } = AddEditAccountTypeResources.ButtonCancelContent;

    public AddEditAccountType()
    {
        using var context = new DataBaseContext();
        AccountTypes = [..context.TAccountTypes];

        InitializeComponent();
    }

    private void TextBoxAccountType_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var accountTypeName = textBox.Text;
        if (string.IsNullOrEmpty(accountTypeName)) return;

        var alreadyExist = CheckAccountTypeName(accountTypeName);
        if (alreadyExist) MessageBox.Show("Account type name already exist");
        else Close();
    }

    private bool CheckAccountTypeName(string accountName)
        => AccountTypes.Select(s => s.Name).Contains(accountName);
}