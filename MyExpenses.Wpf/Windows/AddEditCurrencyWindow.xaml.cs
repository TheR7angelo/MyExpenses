﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditCurrencyWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditCurrencyWindow
{
    #region DepencyProperty

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EditCurrencyProperty = DependencyProperty.Register(nameof(EditCurrency),
        typeof(bool), typeof(AddEditCurrencyWindow), new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EditCurrency
    {
        get => (bool)GetValue(EditCurrencyProperty);
        set => SetValue(EditCurrencyProperty, value);
    }

    #endregion

    #region Property

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TCurrency Currency { get; } = new();

    private List<TCurrency> Currencies { get; }

    public bool CurrencyDeleted { get; private set; }

    #endregion

    #region Resx

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxCurrencySymbolProperty =
        DependencyProperty.Register(nameof(TextBoxCurrencySymbol), typeof(string), typeof(AddEditCurrencyWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxCurrencySymbol
    {
        get => (string)GetValue(TextBoxCurrencySymbolProperty);
        set => SetValue(TextBoxCurrencySymbolProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonValidContentProperty =
        DependencyProperty.Register(nameof(ButtonValidContent), typeof(string), typeof(AddEditCurrencyWindow),
            new PropertyMetadata(default(string)));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonDeleteContentProperty =
        DependencyProperty.Register(nameof(ButtonDeleteContent), typeof(string), typeof(AddEditCurrencyWindow),
            new PropertyMetadata(default(string)));

    public string ButtonDeleteContent
    {
        get => (string)GetValue(ButtonDeleteContentProperty);
        set => SetValue(ButtonDeleteContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(AddEditCurrencyWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(AddEditCurrencyWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    #endregion

    public AddEditCurrencyWindow()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        using var context = new DataBaseContext();
        Currencies = [..context.TCurrencies];

        UpdateLanguage();
        InitializeComponent();
        TextBoxCurrency.Focus();

        this.SetWindowCornerPreference();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        TitleWindow = AddEditCurrencyWindowResources.TitleWindow;

        TextBoxCurrencySymbol = AddEditCurrencyWindowResources.TextBoxCurrencySymbol;
        ButtonValidContent = AddEditCurrencyWindowResources.ButtonValidContent;
        ButtonDeleteContent = AddEditCurrencyWindowResources.ButtonDeleteContent;
        ButtonCancelContent = AddEditCurrencyWindowResources.ButtonCancelContent;
    }

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.MsgBox.Show(AddEditCurrencyWindowResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the currency symbol \"{CurrencyToDeleteSymbol}\"", Currency.Symbol);
        var (success, exception) = Currency.Delete();

        if (success)
        {
            Log.Information("Currency symbol was successfully removed");
            MsgBox.MsgBox.Show(AddEditCurrencyWindowResources.MessageBoxDeleteCurrencyNoUseSuccess, MsgBoxImage.Check);

            CurrencyDeleted = true;
            DialogResult = true;
            Close();
            return;
        }

        if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response = MsgBox.MsgBox.Show(AddEditCurrencyWindowResources.MessageBoxDeleteCurrencyUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response != MessageBoxResult.Yes) return;

            Log.Information(
                "Attempting to remove the currency symbol \"{CurrencyToDeleteSymbol}\" with all relative element",
                Currency.Symbol);
            Currency.Delete(true);
            Log.Information("Currency symbol and all relative element was successfully removed");
            MsgBox.MsgBox.Show(AddEditCurrencyWindowResources.MessageBoxDeleteCurrencyUseSuccess, MsgBoxImage.Check);

            CurrencyDeleted = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.MsgBox.Show(AddEditCurrencyWindowResources.MessageBoxDeleteCurrencyError, MsgBoxImage.Error);
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var currencySymbol = Currency.Symbol;

        if (string.IsNullOrWhiteSpace(currencySymbol))
        {
            MsgBox.MsgBox.Show(AddEditCurrencyWindowResources.MessageBoxCurrencySymbolCannotEmpty, MsgBoxImage.Warning);
            return;
        }

        var alreadyExist = CheckCurrencySymbol(currencySymbol);
        if (alreadyExist) ShowErrorMessage();
        else
        {
            DialogResult = true;
            Close();
        }
    }

    private void TextBoxCurrencySymbol_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var currencySymbol = textBox.Text;
        if (string.IsNullOrEmpty(currencySymbol)) return;

        var alreadyExist = CheckCurrencySymbol(currencySymbol);
        if (alreadyExist) ShowErrorMessage();
    }

    #endregion

    #region Function

    private bool CheckCurrencySymbol(string currencySymbol)
        => Currencies.Select(s => s.Symbol).Contains(currencySymbol);

    public void SetTCurrency(TCurrency currencyToEdit)
    {
        currencyToEdit.CopyPropertiesTo(Currency);
        EditCurrency = true;

        var oldItem = Currencies.FirstOrDefault(s => s.Id == currencyToEdit.Id);
        if (oldItem is null) return;
        Currencies.Remove(oldItem);
    }

    private static void ShowErrorMessage()
        => MsgBox.MsgBox.Show(AddEditCurrencyWindowResources.MessageBoxCurrencySymbolAlreadyExists,
            MsgBoxImage.Warning);

    #endregion
}