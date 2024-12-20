﻿using System.Reflection;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.Smartphones.Resources.Resx.AppShells.DashBoardShell;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.AppShells;

public partial class DashBoardShell
{
    public static readonly BindableProperty FlyoutItemGeneralAccountSetupContentPageTitleProperty =
        BindableProperty.Create(nameof(FlyoutItemGeneralAccountSetupContentPageTitle), typeof(string),
            typeof(DashBoardShell), default(string));

    public string FlyoutItemGeneralAccountSetupContentPageTitle
    {
        get => (string)GetValue(FlyoutItemGeneralAccountSetupContentPageTitleProperty);
        set => SetValue(FlyoutItemGeneralAccountSetupContentPageTitleProperty, value);
    }

    public static readonly BindableProperty FlyoutItemDashBoardContentPageTitleProperty =
        BindableProperty.Create(nameof(FlyoutItemDashBoardContentPageTitle), typeof(string), typeof(DashBoardShell),
            default(string));

    public string FlyoutItemDashBoardContentPageTitle
    {
        get => (string)GetValue(FlyoutItemDashBoardContentPageTitleProperty);
        set => SetValue(FlyoutItemDashBoardContentPageTitleProperty, value);
    }

    public static readonly BindableProperty ApplicationVersionProperty =
        BindableProperty.Create(nameof(ApplicationVersion), typeof(Version), typeof(DashBoardShell), default(Version));

    public Version ApplicationVersion
    {
        get => (Version)GetValue(ApplicationVersionProperty);
        set => SetValue(ApplicationVersionProperty, value);
    }

    public static readonly BindableProperty ApplicationNameProperty =
        BindableProperty.Create(nameof(ApplicationName), typeof(string), typeof(DashBoardShell), default(string));

    public string ApplicationName
    {
        get => (string)GetValue(ApplicationNameProperty);
        set => SetValue(ApplicationNameProperty, value);
    }

    public static readonly BindableProperty MenuItemLogoutTextProperty =
        BindableProperty.Create(nameof(MenuItemLogoutText), typeof(string), typeof(DashBoardShell), default(string));

    public string MenuItemLogoutText
    {
        get => (string)GetValue(MenuItemLogoutTextProperty);
        set => SetValue(MenuItemLogoutTextProperty, value);
    }

    public static readonly BindableProperty SelectedDatabaseProperty = BindableProperty.Create(nameof(SelectedDatabase),
        typeof(ExistingDatabase), typeof(DashBoardShell), default(ExistingDatabase));

    public ExistingDatabase SelectedDatabase
    {
        get => (ExistingDatabase)GetValue(SelectedDatabaseProperty);
        set => SetValue(SelectedDatabaseProperty, value);
    }

    // TODO continue
    public DashBoardShell()
    {
        var assembly = Assembly.GetExecutingAssembly();
        ApplicationName = assembly.GetName().Name!;
        ApplicationVersion = assembly.GetName().Version!;

        UpdateLanguage();

        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void MenuItemLogout_OnClicked(object? sender, EventArgs e)
    {
        DataBaseContext.FilePath = null;

        var appShell = new AppShell();
        Application.Current!.Windows[0].Page = appShell;
    }

    #endregion

    #region Function

    private void UpdateLanguage()
    {
        FlyoutItemDashBoardContentPageTitle = DashBoardShellResources.FlyoutItemDashBoardContentPageTitle;
        FlyoutItemGeneralAccountSetupContentPageTitle = DashBoardShellResources.FlyoutItemGeneralAccountSetupContentPageTitle;
        MenuItemLogoutText = DashBoardShellResources.MenuItemLogoutText;
    }

    #endregion
}