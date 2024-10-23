using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.Smartphones.Resources.Resx.AppShells.DashBoardShell;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.AppShells;

public partial class DashBoardShell
{
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
        UpdateLanguage();

        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;

        //TODO test
        var request = new GeolocationRequest(GeolocationAccuracy.Medium);
        var location = Geolocation.GetLocationAsync(request).GetAwaiter().GetResult();

        var isNorthernHemisphere = location?.Latitude >= 0;
    }

    #region Action

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void MenuItemLogout_OnClicked(object? sender, EventArgs e)
    {
        DataBaseContext.FilePath = null;

        var appShell = new AppShell();
        Application.Current!.MainPage = appShell;
    }

    #endregion

    #region Function

    private void UpdateLanguage()
    {
        MenuItemLogoutText = DashBoardShellResources.MenuItemLogoutText;
    }

    #endregion
}