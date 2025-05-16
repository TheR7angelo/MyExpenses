using System.Reflection;
using System.Runtime.Versioning;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.SharedUtils.Resources.Resx.DashBoardShellManagement;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.AppShells;

public partial class DashBoardShell
{
    public static readonly BindableProperty FlyoutItemGeneralAccountSetupContentPageTitleProperty =
        BindableProperty.Create(nameof(FlyoutItemGeneralAccountSetupContentPageTitle), typeof(string),
            typeof(DashBoardShell));

    public string FlyoutItemGeneralAccountSetupContentPageTitle
    {
        get => (string)GetValue(FlyoutItemGeneralAccountSetupContentPageTitleProperty);
        set => SetValue(FlyoutItemGeneralAccountSetupContentPageTitleProperty, value);
    }

    public static readonly BindableProperty FlyoutItemDashBoardContentPageTitleProperty =
        BindableProperty.Create(nameof(FlyoutItemDashBoardContentPageTitle), typeof(string), typeof(DashBoardShell));

    public string FlyoutItemDashBoardContentPageTitle
    {
        get => (string)GetValue(FlyoutItemDashBoardContentPageTitleProperty);
        set => SetValue(FlyoutItemDashBoardContentPageTitleProperty, value);
    }

    public static readonly BindableProperty ApplicationVersionProperty =
        BindableProperty.Create(nameof(ApplicationVersion), typeof(Version), typeof(DashBoardShell));

    public Version ApplicationVersion
    {
        get => (Version)GetValue(ApplicationVersionProperty);
        init => SetValue(ApplicationVersionProperty, value);
    }

    public static readonly BindableProperty ApplicationNameProperty =
        BindableProperty.Create(nameof(ApplicationName), typeof(string), typeof(DashBoardShell));

    public string ApplicationName
    {
        get => (string)GetValue(ApplicationNameProperty);
        init => SetValue(ApplicationNameProperty, value);
    }

    public static readonly BindableProperty MenuItemLogoutTextProperty =
        BindableProperty.Create(nameof(MenuItemLogoutText), typeof(string), typeof(DashBoardShell));

    public string MenuItemLogoutText
    {
        get => (string)GetValue(MenuItemLogoutTextProperty);
        set => SetValue(MenuItemLogoutTextProperty, value);
    }

    public static readonly BindableProperty SelectedDatabaseProperty = BindableProperty.Create(nameof(SelectedDatabase),
        typeof(ExistingDatabase), typeof(DashBoardShell));

    public ExistingDatabase SelectedDatabase
    {
        get => (ExistingDatabase)GetValue(SelectedDatabaseProperty);
        init => SetValue(SelectedDatabaseProperty, value);
    }

    // TODO continue
    public DashBoardShell()
    {
        var assembly = Assembly.GetExecutingAssembly();
        ApplicationName = assembly.GetName().Name!;
        ApplicationVersion = assembly.GetName().Version!;

        UpdateLanguage();

        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        // Necessary allocation of a new AppShell instance.
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.0")]
    [SupportedOSPlatform("MacCatalyst14.0")]
    [SupportedOSPlatform("Windows")]
    private void ButtonExportDataBase_OnClick(object? sender, EventArgs e)
    {
        var existingDatabase = new ExistingDatabase(DataBaseContext.FilePath!);
        _ = this.HandleButtonExportDataBase(existingDatabase);
        Current.FlyoutIsPresented = false;
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void MenuItemLogout_OnClicked(object? sender, EventArgs e)
    {
        DataBaseContext.FilePath = null;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary allocation of a new AppShell instance.
        // This is required to reset the application's navigation or interface when logging out.
        // AppShell serves as the main entry point for the application, and a new instance ensures a clean state.
        var appShell = new AppShell();
        Application.Current!.Windows[0].Page = appShell;
    }

    #endregion

    #region Function

    private void UpdateLanguage()
    {
        FlyoutItemDashBoardContentPageTitle = DashBoardShellManagementResources.FlyoutItemDashBoardContentPageTitle;
        FlyoutItemGeneralAccountSetupContentPageTitle = DashBoardShellManagementResources.FlyoutItemGeneralAccountSetupContentPageTitle;
        MenuItemLogoutText = DashBoardShellManagementResources.MenuItemLogoutText;
    }

    #endregion
}