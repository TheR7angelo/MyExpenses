using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.Pages.CategoryTypeManagementPage;
using MyExpenses.Wpf.Windows;
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
        CategoryTypes = [..context.TCategoryTypes.Include(s => s.ColorFkNavigation).OrderBy(s => s.Name)];

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

        //TODO work
        Console.WriteLine(categoryType.Name);
    }
}