using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Core;
using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.SharedUtils.Resources.Resx.WelcomeManagement;
using MyExpenses.Sql.Context;
using MyExpenses.WebApi.Dropbox;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.AutoUpdaterGitHub;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class WelcomePage
{
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];

    public WelcomePage()
    {
        ExistingDatabases.RefreshExistingDatabases(ProjectSystem.Wpf);

        InitializeComponent();

        AutoUpdaterGitHub.CheckUpdateGitHub();

        // ReSharper disable once HeapView.DelegateAllocation
        MainWindow.VaccumDatabase += MainWindow_OnVaccumDatabase;
    }

    private void MainWindow_OnVaccumDatabase()
        => _ = ExistingDatabases.CheckExistingDatabaseIsSyncAsync(ProjectSystem.Wpf);

    #region Action

    private void ButtonAddDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of AddDatabaseFileWindow is created to handle the addition of a new database file.
        // The SetExistingDatabase method is called with the ExistingDatabases to provide context or validate against existing entries.
        // ShowDialog() is used to display the window modally and obtain the user's action.
        // If the dialog result is not true (e.g., the user cancels or closes the window), the method exits early.
        var addDatabaseFileWindow = new AddDatabaseFileWindow();
        addDatabaseFileWindow.SetExistingDatabase(ExistingDatabases);

        var result = addDatabaseFileWindow.ShowDialog();

        if (result is not true) return;

        var fileName = addDatabaseFileWindow.DatabaseFilename;
        fileName = Path.ChangeExtension(fileName, DatabaseInfos.Extension);
        var filePath = Path.Combine(DatabaseInfos.LocalDirectoryDatabase, fileName);

        Log.Information("Create new database with name \"{FileName}\"", fileName);

        try
        {
            File.Copy(DatabaseInfos.LocalFilePathDataBaseModel, filePath, true);

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // Necessary instantiation of DataBaseContext to interact with the database.
            // This creates a scoped database context for performing queries and modifications in the database.
            using var context = new DataBaseContext(filePath);
            context.SetAllDefaultValues();
            context.SaveChanges();

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The instantiation of ExistingDatabase is necessary to interact with
            // a specific database identified by its file path (filePath).
            // This ensures that each database is represented by a dedicated instance,
            // allowing proper management and addition to the ExistingDatabases collection.
            var existingDatabase = new ExistingDatabase(filePath);
            ExistingDatabases.AddAndSort(existingDatabase, s => s.FileNameWithoutExtension);

            Log.Information("New database was successfully added");
            MsgBox.Show(WelcomeManagementResources.MessageBoxAddDataBaseSuccessTitle,
                WelcomeManagementResources.MessageBoxAddDataBaseSuccessMessage, MsgBoxImage.Check);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occur");
            MsgBox.Show(WelcomeManagementResources.MessageBoxAddDataBaseErrorTitle,
                WelcomeManagementResources.MessageBoxAddDataBaseErrorMessage, MsgBoxImage.Error);
        }
    }

    private void ButtonDatabase_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not ExistingDatabase existingDatabase) return;

        if (existingDatabase.SyncStatus is SyncStatus.Unknown) existingDatabase.CheckExistingDatabaseIsSync(ProjectSystem.Wpf);
        if (existingDatabase.SyncStatus is SyncStatus.LocalIsOutdated)
        {
            var question = string.Format(WelcomeManagementResources.MessageBoxUseOutdatedWarningQuestionMessage, Environment.NewLine);

            var response = MsgBox.Show(WelcomeManagementResources.MessageBoxUseOutdatedWarningQuestionTitle, question, MessageBoxButton.YesNo, MsgBoxImage.Question);
            if (response is not MessageBoxResult.Yes) return;
        }

        Log.Information("Connection to the database : \"{FileName}\" with statut : {Status}", existingDatabase.FileNameWithoutExtension, existingDatabase.SyncStatus);

        DataBaseContext.FilePath = existingDatabase.FilePath;
        // nameof(MainWindow.FrameBody).NavigateTo(typeof(DashBoard2Page));
        nameof(MainWindow.FrameBody).NavigateTo(typeof(DashBoardPage));
    }

    private void ButtonExportDataBase_OnClick(object sender, RoutedEventArgs e)
        => _ = ExistingDatabases.HandleButtonExportDataBase();

    private void ButtonImportDataBase_OnClick(object sender, RoutedEventArgs e)
        => _ = ExistingDatabases.HandleButtonImportDataBase();

    private void ButtonRemoveDataBase_OnClick(object sender, RoutedEventArgs e)
        => _ = ExistingDatabases.HandleButtonRemoveDataBase();

    #endregion
}