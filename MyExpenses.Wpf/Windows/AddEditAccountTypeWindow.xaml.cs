using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditAccountTypeWindow;
using MyExpenses.Wpf.Windows.MsgBox;

namespace MyExpenses.Wpf.Windows.DashBoardPage;

public partial class AddEditAccountTypeWindow
{
    #region Property

    public TAccountType AccountType { get; } = new();

    private List<TAccountType> AccountTypes { get; }

    #endregion

    #region Resx

    public string TextBoxAccountTypeName { get; } = AddEditAccountTypeWindowResources.TextBoxAccountTypeName;
    public string ButtonValidContent { get; } = AddEditAccountTypeWindowResources.ButtonValidContent;
    public string ButtonCancelContent { get; } = AddEditAccountTypeWindowResources.ButtonCancelContent;

    #endregion

    public AddEditAccountTypeWindow()
    {
        using var context = new DataBaseContext();
        AccountTypes = [..context.TAccountTypes];

        InitializeComponent();
        TextBoxAccountType.Focus();
    }

    #region Function

    private bool CheckAccountTypeName(string accountName)
        => AccountTypes.Select(s => s.Name).Contains(accountName);

    private void ShowErrorMessage()
        => MsgBox.MsgBox.Show(AddEditAccountTypeWindowResources.MessageBoxAccountTypeNameAlreadyExists, MsgBoxImage.Warning);


    #endregion

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var accountTypeName = AccountType.Name;

        if (string.IsNullOrEmpty(accountTypeName)) return;

        var alreadyExist = CheckAccountTypeName(accountTypeName);
        if (alreadyExist) ShowErrorMessage();
        else
        {
            DialogResult = true;
            Close();
        }
    }

    private void TextBoxAccountType_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var accountTypeName = textBox.Text;
        if (string.IsNullOrEmpty(accountTypeName)) return;

        var alreadyExist = CheckAccountTypeName(accountTypeName);
        if (alreadyExist) ShowErrorMessage();
    }

    #endregion
}