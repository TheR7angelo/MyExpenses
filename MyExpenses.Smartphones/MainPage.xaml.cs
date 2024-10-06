using System.Collections.Immutable;
using System.Collections.ObjectModel;
using MyExpenses.Models.IO;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;

namespace MyExpenses.Smartphones;

public partial class MainPage
{
    private int _count;

    public static readonly BindableProperty LabelContentProperty =
        BindableProperty.Create(nameof(LabelContent), typeof(string), typeof(MainPage), default(string));

    public string LabelContent
    {
        get => (string)GetValue(LabelContentProperty);
        set => SetValue(LabelContentProperty, value);
    }

    public static readonly BindableProperty LabelCountContentProperty =
        BindableProperty.Create(nameof(LabelCountContent), typeof(string), typeof(MainPage), "Clicked 0 time");

    public string LabelCountContent
    {
        get => (string)GetValue(LabelCountContentProperty);
        set => SetValue(LabelCountContentProperty, value);
    }

    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];

    public MainPage()
    {
        var dbFilePath = DbContextBackup.LocalFilePathDataBaseModel;
        using var context = new DataBaseContext(dbFilePath);

        LabelContent = context.TVersions.First().Version!.ToString();

        RefreshExistingDatabases();

        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        _count++;

        LabelCountContent = _count is 1
        ? $"Clicked {_count} time"
        : $"Clicked {_count} times";

        SemanticScreenReader.Announce(LabelCountContent);
    }

    private void RefreshExistingDatabases()
    {
        var itemsToDelete = ExistingDatabases
            .Where(s => !File.Exists(s.FilePath)).ToImmutableArray();

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
}