﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.AddEditAccountType;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditAccountType
{
    #region Property

    public TAccountType AccountType { get; } = new();

    public ObservableCollection<TAccountType> AccountTypes { get; }

    #endregion

    #region Resx

    public string TextBoxAccountTypeName { get; } = AddEditAccountTypeResources.TextBoxAccountTypeName;
    public string ButtonValidContent { get; } = AddEditAccountTypeResources.ButtonValidContent;
    public string ButtonDeleteContent { get; } = AddEditAccountTypeResources.ButtonDeleteContent;
    public string ButtonCancelContent { get; } = AddEditAccountTypeResources.ButtonCancelContent;

    #endregion

    public AddEditAccountType()
    {
        using var context = new DataBaseContext();
        AccountTypes = [..context.TAccountTypes];

        InitializeComponent();
    }

    #region Function

    private bool CheckAccountTypeName(string accountName)
        => AccountTypes.Select(s => s.Name).Contains(accountName);

    private void ShowErrorMessage()
        => MessageBox.Show(AddEditAccountTypeResources.MessageBoxAccountTypeNameAlreadyExists);

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