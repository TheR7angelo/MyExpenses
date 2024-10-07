﻿using System.Collections.ObjectModel;
using MyExpenses.Models.IO;
using MyExpenses.Smartphones.UserControls.CustomFrame;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;

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

    private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        var frame = (Frame)sender!;
        if (frame.BindingContext is not ExistingDatabase existingDatabase) return;

        DataBaseContext.FilePath = existingDatabase.FilePath;

        var dashBoardShell = new DashBoardShell();
        Application.Current!.MainPage = dashBoardShell;
    }

    private void ButtonImageView_OnClicked(object? sender, EventArgs e)
    {
        var buttonImageView = (ButtonImageTextView)sender!;
        if (buttonImageView.BindingContext is not ExistingDatabase existingDatabase) return;

        DataBaseContext.FilePath = existingDatabase.FilePath;

        var dashBoardShell = new DashBoardShell();
        Application.Current!.MainPage = dashBoardShell;
    }
}