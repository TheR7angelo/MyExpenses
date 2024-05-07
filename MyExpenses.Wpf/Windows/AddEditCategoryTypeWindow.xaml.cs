using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditCategoryTypeWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditCategoryTypeWindow
{
    public static readonly DependencyProperty EditCategoryTypeProperty =
        DependencyProperty.Register(nameof(EditCategoryType), typeof(bool), typeof(AddEditCategoryTypeWindow),
            new PropertyMetadata(default(bool)));

    public bool EditCategoryType
    {
        get => (bool)GetValue(EditCategoryTypeProperty);
        set => SetValue(EditCategoryTypeProperty, value);
    }

    #region Property

    public TCategoryType CategoryType { get; } = new();
    public ObservableCollection<TColor> Colors { get; }
    private List<TCategoryType> CategoryTypes { get; }

    #endregion

    #region Resx

    public string TextBoxCategoryTypeName { get; } = AddEditCategoryTypeWindowResources.TextBoxCategoryTypeName;
    public string ComboBoxColorValue { get; } = AddEditCategoryTypeWindowResources.ComboBoxColorValue;
    public string ButtonValidContent { get; } = AddEditCategoryTypeWindowResources.ButtonValidContent;
    public string ButtonCancelContent { get; } = AddEditCategoryTypeWindowResources.ButtonCancelContent;

    #endregion

    public string ComboBoxColorDisplayMemberPath { get; } = nameof(TColor.Name);
    public string ComboBoxColorSelectedValuePath { get; } = nameof(TColor.Id);

    public AddEditCategoryTypeWindow()
    {
        using var context = new DataBaseContext();
        CategoryTypes = [..context.TCategoryTypes];
        Colors = [..context.TColors];

        InitializeComponent();
        TextBoxCategoryType.Focus();
    }

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var categoryTypeName = CategoryType.Name;
        if (string.IsNullOrWhiteSpace(categoryTypeName))
        {
            MsgBox.MsgBox.Show(AddEditCategoryTypeWindowResources.MessageBoxCategoryNameCannotBeEmptyError, MsgBoxImage.Error);
            return;
        }

        if (CheckCategoryTypeName(categoryTypeName))
        {
            ShowErrorMessage();
            return;
        }

        if (CategoryType.ColorFk is null)
        {
            MsgBox.MsgBox.Show(AddEditCategoryTypeWindowResources.MessageBoxCategoryColorCannotBeEmptyError, MsgBoxImage.Error);
            return;
        }

        DialogResult = true;
        Close();
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

    #region Function

    private bool CheckCategoryTypeName(string accountName)
        => CategoryTypes.Select(s => s.Name).Contains(accountName);

    public void SetTCategoryType(TCategoryType categoryType)
    {
        categoryType.CopyPropertiesTo(CategoryType);
        EditCategoryType = true;
        CategoryTypes.Remove(CategoryTypes.Find(s => s.Id == categoryType.Id)!);
    }

    private void ShowErrorMessage()
        => MsgBox.MsgBox.Show(AddEditCategoryTypeWindowResources.MessageBoxCategoryAlreadyExists, MsgBoxImage.Warning);

    #endregion

    private void ButtonAddColor_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
        Console.WriteLine("Need to create color");
    }
}