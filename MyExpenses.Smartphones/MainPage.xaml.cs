﻿using MyExpenses.Sql.Context;

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

    public MainPage()
    {
        var stream = FileSystem.Current.OpenAppPackageFileAsync(Path.Join("Database Models", "Model.sqlite"))
            .ConfigureAwait(false).GetAwaiter().GetResult();
        var databaseFilePath = Path.Join(FileSystem.AppDataDirectory, "Database Models");
        Directory.CreateDirectory(databaseFilePath);
        databaseFilePath = Path.Join(databaseFilePath, "Model.sqlite");

        using var fileStream = File.Create(databaseFilePath);
        stream.CopyTo(fileStream);

        var dbFilePath = Path.Join(FileSystem.AppDataDirectory, "Database Models", "Model.sqlite");
        using var context = new DataBaseContext(dbFilePath);

        LabelContent = context.TVersions.First().Version!.ToString();

        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        _count++;

        CounterBtn.Text = _count is 1
            ? $"Clicked {_count} time"
            : $"Clicked {_count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}