using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.Pages.CategoryTypeManagementPage;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class CategoryTypeManagementPage
{
    public ObservableCollection<TCategoryType> CategoryTypes { get; }

    public required DashBoardPage DashBoardPage { get; init; }

    public CategoryTypeManagementPage()
    {
        using var context = new DataBaseContext();

        CategoryTypes = new ObservableCollection<TCategoryType>();

        var temp = context.TCategoryTypes.OrderBy(s => s.Name).ToList();
        var colors = context.TColors.AsNoTracking().ToDictionary(c => c.Id);

        foreach (var t in temp)
        {
            if (t.ColorFk is not null)
            {
                t.ColorFkNavigation = colors.TryGetValue((int)t.ColorFk!, out var color) ? color : null;
            }
            CategoryTypes.Add(t);
        }

        InitializeComponent();
    }

    private void ButtonAddCategoryType_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditCategoryType = new AddEditCategoryTypeWindow();
        var result = addEditCategoryType.ShowDialog();
        if (result != true) return;

        var newCategoryType = addEditCategoryType.CategoryType;
        Log.Information("Attempting to inject the new category type \"{NewCategoryTypeName}\"", newCategoryType.Name);
        var (success, exception) = newCategoryType.AddOrEdit();
        if (success)
        {
            CategoryTypes.AddAndSort(newCategoryType, s => s.Name!);
            Log.Information("Account type was successfully added");
            MsgBox.Show(CategoryTypeManagementPageResources.MessageBoxAddCategorySuccess, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(CategoryTypeManagementPageResources.MessageBoxAddCategoryError, MsgBoxImage.Error);
        }
    }

    private void ButtonEditCategoryType_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not TCategoryType categoryType) return;

        var addEditCategoryTypeWindow = new AddEditCategoryTypeWindow();
        addEditCategoryTypeWindow.SetTCategoryType(categoryType);
        var result = addEditCategoryTypeWindow.ShowDialog();
        if (result != true) return;

        var editedCategoryType = addEditCategoryTypeWindow.CategoryType;
        Log.Information("Attempting to edit the category type id: {Id}", editedCategoryType.Id);

        var editedCategoryTypeDeepCopy = editedCategoryType.DeepCopy();

        var (success, exception) = editedCategoryTypeDeepCopy.AddOrEdit();
        if (success)
        {
            using var context = new DataBaseContext();
            editedCategoryTypeDeepCopy.ColorFkNavigation =
                context.TColors.FirstOrDefault(s => s.Id == editedCategoryTypeDeepCopy.ColorFk);

            CategoryTypes.AddAndSort(categoryType, editedCategoryTypeDeepCopy, s => s.Name!);

            var radioButton = DashBoardPage.ItemsControlVTotalAccount
                .FindVisualChildren<RadioButton>()
                .FirstOrDefault(s => (bool)s.IsChecked!);
            if (radioButton is not null) DashBoardPage.UpdateGraph((string)radioButton.Content, DateTime.Now);

            Log.Information("Category type was successfully edited");
            MsgBox.Show(CategoryTypeManagementPageResources.MessageBoxEditCategorySuccess, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(CategoryTypeManagementPageResources.MessageBoxEditCategoryError, MsgBoxImage.Error);
        }
    }
}