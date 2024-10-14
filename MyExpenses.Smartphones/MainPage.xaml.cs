﻿using System.Collections.ObjectModel;
using MyExpenses.Models.IO;
using MyExpenses.Smartphones.AppShells;
using MyExpenses.Smartphones.ContentPages;
using MyExpenses.Smartphones.UserControls.Buttons.CustomFrame;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using Serilog;

namespace MyExpenses.Smartphones;

public partial class MainPage
{
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];

    public MainPage()
    {
        RefreshExistingDatabases();

        InitializeComponent();
    }

    #region Function

    private void RefreshExistingDatabases()
    {
        var itemsToDelete = ExistingDatabases
            .Where(s => !File.Exists(s.FilePath)).AsEnumerable();

        foreach (var item in itemsToDelete)
        {
            ExistingDatabases.Remove(item);
        }

        var newExistingDatabases = DbContextBackup.GetExistingDatabase();
        foreach (var existingDatabase in newExistingDatabases)
        {
            var exist = ExistingDatabases.FirstOrDefault(s => s.FilePath == existingDatabase.FilePath);
            if (exist is not null)
            {
                existingDatabase.CopyPropertiesTo(exist);
            }
            else
            {
                ExistingDatabases.AddAndSort(existingDatabase, s => s.FileNameWithoutExtension);
            }
        }
    }

    #endregion

    #region Action

    private void ButtonDatabase_OnClick(object? sender, EventArgs e)
    {
        var buttonImageView = (ButtonImageTextView)sender!;
        if (buttonImageView.BindingContext is not ExistingDatabase existingDatabase) return;

        DataBaseContext.FilePath = existingDatabase.FilePath;

        var dashBoardShell = new DashBoardShell();
        Application.Current!.MainPage = dashBoardShell;
    }

    #endregion

    private async void ButtonAddDataBase_OnClick(object? sender, EventArgs e)
    {
        var addDatabaseFileContentPage = new AddDatabaseFileContentPage();
        addDatabaseFileContentPage.SetExistingDatabase(ExistingDatabases);

        await Navigation.PushAsync(addDatabaseFileContentPage);

        var result = await addDatabaseFileContentPage.ResultDialog;

        if (result is not true) return;

        var fileName = addDatabaseFileContentPage.DatabaseFilename;
        fileName = Path.ChangeExtension(fileName, ".sqlite");
        var filePath = Path.Combine(DbContextBackup.LocalDirectoryDatabase, fileName);

        Log.Information("Create new database with name \"{FileName}\"", fileName);

        try
        {
            File.Copy(DbContextBackup.LocalFilePathDataBaseModel, filePath, true);

            await using var context = new DataBaseContext(filePath);
            context.SetAllDefaultValues();
            await context.SaveChangesAsync();

            ExistingDatabases.AddAndSort(new ExistingDatabase(filePath),
                s => s.FileNameWithoutExtension);

            //TODO trad
            await DisplayAlert("Success", "Success", "Ok");
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occur");

            //TODO trad
            await DisplayAlert("An error occur", "An error occur", "Ok");
        }
    }

    private async void ButtonRemoveDataBase_OnClick(object? sender, EventArgs e)
    {
        var selectDatabaseFileContentPage = new SelectDatabaseFileContentPage();
        selectDatabaseFileContentPage.ExistingDatabases.AddRange(ExistingDatabases);

        await Navigation.PushAsync(selectDatabaseFileContentPage);

        var result = await selectDatabaseFileContentPage.ResultDialog;

        if (result is not true) return;

        //TODO trad
        var response = await DisplayAlert("Question", "Do you really want to delete all database selected ?", "Accept",
            "Cancel");

        if (!response) return;

        foreach (var existingDatabase in selectDatabaseFileContentPage.ExistingDatabasesSelected)
        {
            if (string.IsNullOrEmpty(existingDatabase.FilePath)) continue;
            if (!File.Exists(existingDatabase.FilePath)) continue;

            File.Delete(existingDatabase.FilePath);

            var backupDirectory = Path.Join(DbContextBackup.LocalDirectoryBackupDatabase, existingDatabase.FileNameWithoutExtension);
            if (Directory.Exists(backupDirectory)) Directory.Delete(backupDirectory, true);
        }

        RefreshExistingDatabases();

        //TODO dropbox connexion
        // response = MsgBox.Show(WelcomePageResources.MessageBoxDeleteCloudQuestion, MsgBoxImage.Question,
        //     MessageBoxButton.YesNoCancel);
        //
        // if (response is MessageBoxResult.Yes)
        // {
        //     var files = selectDatabaseFileWindow.ExistingDatabasesSelected.Select(s => s.FileName).ToArray();
        //     Log.Information("Preparing to delete the following files: {Files}", files);
        //
        //     var dropboxService = new DropboxService();
        //     await dropboxService.DeleteFilesAsync(files, DbContextBackup.CloudDirectoryBackupDatabase);
        // }
        //
        // Log.Information("Files successfully deleted");
        // MsgBox.Show(WelcomePageResources.MessageBoxDeleteCloudQuestionSuccess, MsgBoxImage.Check, MessageBoxButton.OK);
    }

    private void ButtonImportDataBase_OnClick(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ButtonExportDataBase_OnClick(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}