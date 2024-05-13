﻿using System.Collections.ObjectModel;
using System.Windows;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Pages;

public partial class AccountTypeManagementPage
{
    public ObservableCollection<TAccountType> AccountTypes { get; }

    public required DashBoardPage DashBoardPage { get; init; }

    public AccountTypeManagementPage()
    {
        using var context = new DataBaseContext();
        AccountTypes = [..context.TAccountTypes.OrderBy(s => s.Name)];

        InitializeComponent();
    }

    //TODO work
    private void ButtonAddNewAccountType_OnClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Add new acount type");
    }

    //TODO work
    private void ButtonAccountType_OnClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Add new acount type");
    }
}