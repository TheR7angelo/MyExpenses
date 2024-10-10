using System.Collections.ObjectModel;
using MyExpenses.Models.IO;
using MyExpenses.Smartphones.ContentPages;
using MyExpenses.Smartphones.UserControls.CustomFrame;
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
    }

    private void ButtonRemoveDataBase_OnClick(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
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