﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.CategoryTypesManagement;
using MyExpenses.SharedUtils.Resources.Resx.ColorManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;

public partial class AddEditCategoryTypeWindow
{
    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EditCategoryTypeProperty =
        DependencyProperty.Register(nameof(EditCategoryType), typeof(bool), typeof(AddEditCategoryTypeWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EditCategoryType
    {
        get => (bool)GetValue(EditCategoryTypeProperty);
        set => SetValue(EditCategoryTypeProperty, value);
    }

    #region Property

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TCategoryType CategoryType { get; } = new();
    public ObservableCollection<TColor> Colors { get; }
    private List<TCategoryType> CategoryTypes { get; }

    public bool CategoryTypeDeleted { get; private set; }

    #endregion

    #region Resx

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(AddEditCategoryTypeWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxCategoryTypeNameProperty =
        DependencyProperty.Register(nameof(TextBoxCategoryTypeName), typeof(string), typeof(AddEditCategoryTypeWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxCategoryTypeName
    {
        get => (string)GetValue(TextBoxCategoryTypeNameProperty);
        set => SetValue(TextBoxCategoryTypeNameProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ComboBoxColorValueProperty =
        DependencyProperty.Register(nameof(ComboBoxColorValue), typeof(string), typeof(AddEditCategoryTypeWindow),
            new PropertyMetadata(default(string)));

    public string ComboBoxColorValue
    {
        get => (string)GetValue(ComboBoxColorValueProperty);
        set => SetValue(ComboBoxColorValueProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonValidContentProperty =
        DependencyProperty.Register(nameof(ButtonValidContent), typeof(string), typeof(AddEditCategoryTypeWindow),
            new PropertyMetadata(default(string)));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonDeleteContentProperty =
        DependencyProperty.Register(nameof(ButtonDeleteContent), typeof(string), typeof(AddEditCategoryTypeWindow),
            new PropertyMetadata(default(string)));

    public string ButtonDeleteContent
    {
        get => (string)GetValue(ButtonDeleteContentProperty);
        set => SetValue(ButtonDeleteContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(AddEditCategoryTypeWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    #endregion

    public string ComboBoxColorSelectedValuePath { get; } = nameof(TColor.Id);

    public AddEditCategoryTypeWindow()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        CategoryTypes = [..context.TCategoryTypes];
        Colors = [..context.TColors.OrderBy(s => s.Name)];

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;

        this.SetWindowCornerPreference();
    }

    private void UpdateLanguage()
    {
        TitleWindow = CategoryTypesManagementResources.TitleWindow;

        TextBoxCategoryTypeName = CategoryTypesManagementResources.TextBoxCategoryTypeName;
        ComboBoxColorValue = CategoryTypesManagementResources.ComboBoxColorValue;
        ButtonValidContent = CategoryTypesManagementResources.ButtonValidText;
        ButtonDeleteContent = CategoryTypesManagementResources.ButtonDeleteText;
        ButtonCancelContent = CategoryTypesManagementResources.ButtonCancelText;
    }

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the category type \"{CategoryTypeToDeleteName}\"", CategoryType.Name);
        var (success, exception) = CategoryType.Delete();

        if (success)
        {
            Log.Information("Category type was successfully removed");
            MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxDeleteCategoryTypeNoUseSuccess,
                MsgBoxImage.Check);

            CategoryTypeDeleted = true;
            DialogResult = true;

            Close();
            return;
        }

        if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response = MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxDeleteCategoryTypeUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information(
                "Attempting to remove the category type \"{CategoryTypeToDeleteName}\" with all relative element",
                CategoryType.Name);
            CategoryType.Delete(true);
            Log.Information("Category type and all relative element was successfully removed");
            MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxDeleteCategoryTypeUseSuccess,
                MsgBoxImage.Check);

            CategoryTypeDeleted = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxCategoryTypeDeleteErrorMessage, MsgBoxImage.Error);
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var categoryTypeName = CategoryType.Name;
        if (string.IsNullOrWhiteSpace(categoryTypeName))
        {
            MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxValidateCategoryTypeErrorEmptyMessage,
                MsgBoxImage.Error);
            return;
        }

        if (CheckCategoryTypeName(categoryTypeName))
        {
            ShowErrorMessage();
            return;
        }

        if (CategoryType.ColorFk is null)
        {
            MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxValidateColorErrorEmptyMessage,
                MsgBoxImage.Error);
            return;
        }

        DialogResult = true;
        Close();
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void TextBoxCategoryType_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var categoryTypeName = textBox.Text;
        if (string.IsNullOrEmpty(categoryTypeName)) return;

        var alreadyExist = CheckCategoryTypeName(categoryTypeName);
        if (alreadyExist) ShowErrorMessage();
    }

    #endregion

    #region Function

    private bool CheckCategoryTypeName(string accountName)
        => CategoryTypes.Select(s => s.Name).Contains(accountName);

    // ReSharper disable once HeapView.ClosureAllocation
    public void SetTCategoryType(TCategoryType categoryType)
    {
        categoryType.CopyPropertiesTo(CategoryType);
        EditCategoryType = true;

        // ReSharper disable once HeapView.DelegateAllocation
        CategoryTypes.Remove(CategoryTypes.Find(s => s.Id.Equals(categoryType.Id))!);
    }

    private static void ShowErrorMessage()
        => MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxValidateCategoryTypeErrorAlreadyExistMessage, MsgBoxImage.Warning);

    #endregion

    private void ButtonAddColor_OnClick(object sender, RoutedEventArgs e)
    {
        if (CategoryType.ColorFk is not null) EditColor();
        else CreateNewColor();
    }

    private void EditColor()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var addEditColorWindow = new AddEditColorWindow();
        addEditColorWindow.SetTColor((int)CategoryType.ColorFk!);

        addEditColorWindow.ShowDialog();
        if (addEditColorWindow.DialogResult is not true) return;
        if (addEditColorWindow.DeleteColor)
        {
            // ReSharper disable once HeapView.DelegateAllocation
            var colorDeleted = Colors.FirstOrDefault(s => s.Id == CategoryType.ColorFk);
            if (colorDeleted is not null) Colors.Remove(colorDeleted);

            return;
        }

        // ReSharper disable once HeapView.ClosureAllocation
        var editedColor = addEditColorWindow.Color;

        Log.Information("Attempting to edit the color \"{AccountName}\"", editedColor.Name);
        var (success, exception) = editedColor.AddOrEdit();
        if (success)
        {
            Log.Information("Color was successfully edited");
            var json = editedColor.ToJsonString();
            Log.Information("{Json}", json);

            // ReSharper disable once HeapView.DelegateAllocation
            var oldColor = Colors.First(s => s.Id.Equals(editedColor.Id));
            editedColor.CopyPropertiesTo(oldColor);

            MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxEditColorSuccessTitle,
                ColorManagementResources.MessageBoxEditColorSuccessMessage, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxEditColorErrorTitle,
                ColorManagementResources.MessageBoxEditColorErrorMessage, MsgBoxImage.Warning);
        }
    }

    private void CreateNewColor()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var addEditColorWindow = new AddEditColorWindow();
        addEditColorWindow.ShowDialog();

        if (addEditColorWindow.DialogResult is not true) return;

        var newColor = addEditColorWindow.Color;

        Log.Information(
            "Attempt to inject the new color \"{ColorName}\" with hexadecimal code \"{ColorHexadecimalColorCode}\"",
            newColor.Name, newColor.HexadecimalColorCode);

        var (success, exception) = newColor.AddOrEdit();
        if (success)
        {
            Log.Information("color was successfully added");
            var json = newColor.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxAddColorSuccessMessage, MsgBoxImage.Check);

            Colors.AddAndSort(newColor, s => s.Name!);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxAddColorErrorTitle,
                ColorManagementResources.MessageBoxAddColorErrorMessage, MsgBoxImage.Error);
        }
    }
}