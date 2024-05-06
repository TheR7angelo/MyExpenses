using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditCategoryTypeWindow;
using MyExpenses.Wpf.Windows.MsgBox;

namespace MyExpenses.Wpf.Windows.DashBoardPage;

public partial class AddEditCategoryTypeWindow
{
    #region Property

    public TCategoryType CategoryType { get; } = new();

    private List<TCategoryType> CategoryTypes { get; }

    #endregion

    #region Resx

    public string TextBoxCategoryTypeName { get; } = AddEditCategoryTypeWindowResources.TextBoxCategoryTypeName;
    public string ButtonValidContent { get; } = AddEditCategoryTypeWindowResources.ButtonValidContent;
    public string ButtonCancelContent { get; } = AddEditCategoryTypeWindowResources.ButtonCancelContent;

    #endregion

    public AddEditCategoryTypeWindow()
    {
        using var context = new DataBaseContext();
        CategoryTypes = [..context.TCategoryTypes];

        InitializeComponent();
        TextBoxCategoryType.Focus();
    }

    #region Function

    private bool CheckCategoryTypeName(string accountName)
        => CategoryTypes.Select(s => s.Name).Contains(accountName);

    private void ShowErrorMessage()
        => MsgBox.MsgBox.Show(AddEditCategoryTypeWindowResources.MessageBoxCategoryAlreadyExists, MsgBoxImage.Warning);


    #endregion

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var categoryTypeName = CategoryType.Name;

        if (string.IsNullOrEmpty(categoryTypeName)) return;

        var alreadyExist = CheckCategoryTypeName(categoryTypeName);
        if (alreadyExist) ShowErrorMessage();
        else
        {
            DialogResult = true;
            Close();
        }
    }

    private void TextBoxCategoryType_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var categoryTypeName = textBox.Text;
        if (string.IsNullOrEmpty(categoryTypeName)) return;

        var alreadyExist = CheckCategoryTypeName(categoryTypeName);
        if (alreadyExist) ShowErrorMessage();
    }

    #endregion
}