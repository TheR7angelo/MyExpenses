using System.Collections.ObjectModel;
using System.Runtime.Versioning;
using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.SharedUtils.Resources.Resx.WelcomeManagement;
using MyExpenses.Smartphones.AppShells;
using MyExpenses.Smartphones.ContentPages;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Smartphones.UserControls.Buttons.UraniumButtonView;
using MyExpenses.Sql.Context;
using MyExpenses.WebApi.Dropbox;
using Serilog;

namespace MyExpenses.Smartphones;

public partial class MainPage
{
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];

    public MainPage()
    {
        ExistingDatabases.RefreshExistingDatabases(ProjectSystem.Maui);

        InitializeComponent();
    }

    #region Action

    private void ButtonAddDataBase_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonAddDataBase();

    private async void ButtonDatabase_OnClick(object? sender, EventArgs e)
    {
        var buttonImageView = (UraniumButtonImageTextView)sender!;
        if (buttonImageView.BindingContext is not ExistingDatabase existingDatabase) return;

        if (existingDatabase.SyncStatus is SyncStatus.Unknown) existingDatabase.CheckExistingDatabaseIsSync(ProjectSystem.Maui);
        if (existingDatabase.SyncStatus is SyncStatus.LocalIsOutdated)
        {
            var question = string.Format(WelcomeManagementResources.MessageBoxUseOutdatedWarningQuestionMessage, Environment.NewLine);

            var response = await DisplayAlert(WelcomeManagementResources.MessageBoxUseOutdatedWarningQuestionTitle, question,
                WelcomeManagementResources.MessageBoxUseOutdatedWarningQuestionYesButton, WelcomeManagementResources.MessageBoxUseOutdatedWarningQuestionCancelButton);
            if (response is not true) return;
        }

        var message = string.Format(WelcomeManagementResources.ActivityIndicatorOpenDatabase, existingDatabase.FileNameWithoutExtension);
        this.ShowCustomPopupActivityIndicator(message);

        DataBaseContext.FilePath = existingDatabase.FilePath;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Create a new instance of DashBoardShell and assign the selected database.
        // This action allocates memory for the new shell object and switches the application to the new shell.
        var dashBoardShell = new DashBoardShell { SelectedDatabase = existingDatabase };
        Application.Current!.Windows[0].Page = dashBoardShell;
    }

    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.0")]
    [SupportedOSPlatform("MacCatalyst14.0")]
    [SupportedOSPlatform("Windows")]
    private void ButtonExportDataBase_OnClick(object? sender, EventArgs e)
        => _ = this.HandleButtonExportDataBase(ExistingDatabases);

    private void ButtonImportDataBase_OnClick(object? sender, EventArgs e)
        => _ = this.HandleButtonImportDataBase(ExistingDatabases, ProjectSystem.Maui);

    private void ButtonRemoveDataBase_OnClick(object? sender, EventArgs e)
        => _ = this.HandleButtonRemoveDataBase(ExistingDatabases, ProjectSystem.Maui);

    #endregion

    #region Function

    private async Task HandleButtonAddDataBase()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Instantiates a new page of type AddDatabaseFileContentPage, likely used to manage or display database file content.
        var addDatabaseFileContentPage = new AddDatabaseFileContentPage();
        addDatabaseFileContentPage.SetExistingDatabase(ExistingDatabases);

        await addDatabaseFileContentPage.NavigateToAsync();

        var result = await addDatabaseFileContentPage.ResultDialog;

        if (result is not true) return;

        this.ShowCustomPopupActivityIndicator(WelcomeManagementResources.ActivityIndicatorCreateNewDatabase);

        var fileName = addDatabaseFileContentPage.DatabaseFilename;
        fileName = Path.ChangeExtension(fileName, ".sqlite");
        var filePath = Path.Combine(DatabaseInfos.LocalDirectoryDatabase, fileName);

        Log.Information("Create new database with name \"{FileName}\"", fileName);

        try
        {
            File.Copy(DatabaseInfos.LocalFilePathDataBaseModel, filePath, true);


            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // Asynchronously initializes a new instance of DataBaseContext for interacting with the database at the specified file path.
            await using var context = new DataBaseContext(filePath);

            _ = context.SetAllDefaultValues();
            await context.SaveChangesAsync();

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // Adds a new instance of ExistingDatabase, created with the specified file path, to the ExistingDatabases collection
            // and sorts it based on the FileNameWithoutExtension property.
            ExistingDatabases.AddAndSort(new ExistingDatabase(filePath),
                s => s.FileNameWithoutExtension);

            CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
            Log.Information("New database was successfully added");
            await DisplayAlert(WelcomeManagementResources.MessageBoxAddDataBaseSuccessTitle, WelcomeManagementResources.MessageBoxAddDataBaseSuccessMessage, WelcomeManagementResources.MessageBoxAddDataBaseOkButton);
        }
        catch (Exception exception)
        {
            CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
            Log.Error(exception, "An error occur");

            await DisplayAlert(WelcomeManagementResources.MessageBoxAddDataBaseErrorTitle, WelcomeManagementResources.MessageBoxAddDataBaseErrorMessage, WelcomeManagementResources.MessageBoxAddDataBaseOkButton);
        }
    }

    #endregion
}