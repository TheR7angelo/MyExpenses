using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Objects;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Resources.Resx.Pages.CategoryTypeManagementPage;
using MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class CategoryTypeManagementPage
{
    public ObservableCollection<TCategoryType> CategoryTypes { get; }

    public CategoryTypeManagementPage()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
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
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The instance of AddEditCategoryTypeWindow is created locally within this method and is used temporarily.
        // Since there are no references to it after this scope and the Garbage Collector will handle
        // its cleanup efficiently, this allocation is intentional and does not require further optimization.
        var addEditCategoryType = new AddEditCategoryTypeWindow();
        var result = addEditCategoryType.ShowDialog();
        if (result is not true) return;

        var newCategoryType = addEditCategoryType.CategoryType;
        Log.Information("Attempting to inject the new category type \"{NewCategoryTypeName}\"", newCategoryType.Name);
        var (success, exception) = newCategoryType.AddOrEdit();
        if (success)
        {
            CategoryTypes.AddAndSort(newCategoryType, s => s.Name!);

            Log.Information("Account type was successfully added");
            var json = newCategoryType.ToJsonString();
            Log.Information("{Json}", json);

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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The instance of AddEditCategoryTypeWindow is created locally within this method and is used temporarily.
        // Since there are no references to it after this scope and the Garbage Collector will handle
        // its cleanup efficiently, this allocation is intentional and does not require further optimization.
        var addEditCategoryTypeWindow = new AddEditCategoryTypeWindow();
        addEditCategoryTypeWindow.SetTCategoryType(categoryType);
        var result = addEditCategoryTypeWindow.ShowDialog();
        if (result is not true) return;

        if (addEditCategoryTypeWindow.CategoryTypeDeleted)
        {
            CategoryTypes.Remove(categoryType);
            return;
        }

        var editedCategoryType = addEditCategoryTypeWindow.CategoryType;
        Log.Information("Attempting to edit the category type id: {Id}", editedCategoryType.Id);

        var editedCategoryTypeDeepCopy = editedCategoryType.DeepCopy()!;

        var (success, exception) = editedCategoryTypeDeepCopy.AddOrEdit();
        if (success)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // Necessary instantiation of DataBaseContext to interact with the database.
            // This creates a scoped database context for performing queries and modifications in the database.
            using var context = new DataBaseContext();
            editedCategoryTypeDeepCopy.ColorFkNavigation =
                context.TColors.FirstOrDefault(s => s.Id == editedCategoryTypeDeepCopy.ColorFk);

            CategoryTypes.AddAndSort(categoryType, editedCategoryTypeDeepCopy, s => s.Name!);

            Log.Information("Category type was successfully edited");
            var json = editedCategoryTypeDeepCopy.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.Show(CategoryTypeManagementPageResources.MessageBoxEditCategorySuccess, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(CategoryTypeManagementPageResources.MessageBoxEditCategoryError, MsgBoxImage.Error);
        }
    }
}