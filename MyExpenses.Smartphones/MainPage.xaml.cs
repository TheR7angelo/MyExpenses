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

    public static readonly BindableProperty LabelCountContentProperty =
        BindableProperty.Create(nameof(LabelCountContent), typeof(string), typeof(MainPage), "Clicked 0 time");

    public string LabelCountContent
    {
        get => (string)GetValue(LabelCountContentProperty);
        set => SetValue(LabelCountContentProperty, value);
    }

    public MainPage()
    {
        var dbFilePath = DbContextBackup.LocalFilePathDataBaseModel;
        using var context = new DataBaseContext(dbFilePath);

        LabelContent = context.TVersions.First().Version!.ToString();

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
}