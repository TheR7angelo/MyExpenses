﻿using MyExpenses.Smartphones.ColorManipulation;
using MyExpenses.Sql.Context;

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
        var dbFilePath = Path.Join(FileSystem.AppDataDirectory, "Database Models", "Model.sqlite");
        using var context = new DataBaseContext(dbFilePath);

        LabelContent = context.TVersions.First().Version!.ToString();

        // var themeManager = new ThemeManager();
        // themeManager.ApplyTheme(AppTheme.Light);
        //
        // themeManager.SetThemeColor("Primary", Colors.Blue,Colors.Aquamarine);
        //
        InitializeComponent();

        var primaryMid = Colors.DarkRed;
        var primaryLight = primaryMid.Lighten();
        var primaryDark = primaryMid.Darken();
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