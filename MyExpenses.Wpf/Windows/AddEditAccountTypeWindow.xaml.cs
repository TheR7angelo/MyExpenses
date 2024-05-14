﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditAccountTypeWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditAccountTypeWindow
{
    public static readonly DependencyProperty EditAccountTypeProperty =
        DependencyProperty.Register(nameof(EditAccountType), typeof(bool), typeof(AddEditAccountTypeWindow),
            new PropertyMetadata(default(bool)));

    #region Property

    public TAccountType AccountType { get; } = new();

    private List<TAccountType> AccountTypes { get; }

    public bool EditAccountType
    {
        get => (bool)GetValue(EditAccountTypeProperty);
        set => SetValue(EditAccountTypeProperty, value);
    }

    #endregion

    #region Resx

    public string TextBoxAccountTypeName { get; } = AddEditAccountTypeWindowResources.TextBoxAccountTypeName;
    public string ButtonValidContent { get; } = AddEditAccountTypeWindowResources.ButtonValidContent;
    public string ButtonDeleteContent { get; } = AddEditAccountTypeWindowResources.ButtonDeleteContent;
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
        => MsgBox.MsgBox.Show(AddEditAccountTypeWindowResources.MessageBoxAccountTypeNameAlreadyExists,
            MsgBoxImage.Warning);

    #endregion

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    //TODO work
    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Work in progress");
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

    public void SetTAccountType(TAccountType accountType)
    {
        accountType.CopyPropertiesTo(AccountType);
        EditAccountType = true;

        var oldItem = AccountTypes.FirstOrDefault(s => s.Id == accountType.Id);
        if (oldItem is null) return;
        AccountTypes.Remove(oldItem);
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