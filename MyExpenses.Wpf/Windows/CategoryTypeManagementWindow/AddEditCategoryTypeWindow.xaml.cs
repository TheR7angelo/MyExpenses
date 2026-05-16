using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Application.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Systems;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.CategoryTypesManagement;
using MyExpenses.SharedUtils.Resources.Resx.ColorManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Sql;
using Serilog;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;

public partial class AddEditCategoryTypeWindow : IClosable
{
    // // ReSharper disable once HeapView.BoxingAllocation
    // // ReSharper disable once HeapView.ObjectAllocation.Evident
    // public static readonly DependencyProperty IsEditCategoryTypeProperty =
    //     DependencyProperty.Register(nameof(IsEditCategoryType), typeof(bool), typeof(AddEditCategoryTypeWindow),
    //         new PropertyMetadata(false));
    //
    // // ReSharper disable once HeapView.BoxingAllocation
    // public bool IsEditCategoryType
    // {
    //     get => (bool)GetValue(IsEditCategoryTypeProperty);
    //     set => SetValue(IsEditCategoryTypeProperty, value);
    // }

    #region Property

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TCategoryType CategoryType { get; } = new();

    // public CategoryTypeViewModel CategoryTypeViewModel { get; } = new();
    //
    // public ObservableCollection<ColorViewModel> ColorViewModels { get; } = [];

    public bool CategoryTypeDeleted { get; private set; }

    #endregion

    private CategoryTypeManagementViewModel ViewModel => (CategoryTypeManagementViewModel)DataContext;

    public AddEditCategoryTypeWindow(CategoryTypeManagementViewModel vm)
    {


        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // Necessary instantiation of DataBaseContext to interact with the database.
        // // This creates a scoped database context for performing queries and modifications in the database.
        // using var context = new DataBaseContextOld();
        // CategoryTypes = [..context.TCategoryTypes];
        // Colors = [..context.TColors.OrderBy(s => s.Name)];

        InitializeComponent();

        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadAllColorCommand.ExecuteAsync(null);
    }

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = Dialogs.MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the category type \"{CategoryTypeToDeleteName}\"", CategoryType.Name);
        var (success, exception) = CategoryType.Delete();

        if (success)
        {
            Log.Information("Category type was successfully removed");
            Dialogs.MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxDeleteCategoryTypeNoUseSuccess,
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

            response = Dialogs.MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxDeleteCategoryTypeUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information(
                "Attempting to remove the category type \"{CategoryTypeToDeleteName}\" with all relative element",
                CategoryType.Name);
            CategoryType.Delete(true);
            Log.Information("Category type and all relative element was successfully removed");
            Dialogs.MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxDeleteCategoryTypeUseSuccess,
                MsgBoxImage.Check);

            CategoryTypeDeleted = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        Dialogs.MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxCategoryTypeDeleteErrorMessage, MsgBoxImage.Error);
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO check valid

        // _systemPresentationService.CreateCategoryTypeViewModel()

        // var categoryTypeName = CategoryType.Name;
        // if (string.IsNullOrWhiteSpace(categoryTypeName))
        // {
        //     Dialogs.MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxValidateCategoryTypeErrorEmptyMessage,
        //         MsgBoxImage.Error);
        //     return;
        // }
        //
        // if (CheckCategoryTypeName(categoryTypeName))
        // {
        //     ShowErrorMessage();
        //     return;
        // }
        //
        // // TODO correct
        // // if (CategoryType.ColorFk is null)
        // // {
        // //     MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxValidateColorErrorEmptyMessage,
        // //         MsgBoxImage.Error);
        // //     return;
        // // }
        //
        // DialogResult = true;
        // Close();
    }

    #endregion

    #region Function

    // private bool CheckCategoryTypeName(string accountName)
        // => CategoryTypes.Select(s => s.Name).Contains(accountName);

    public void LoadCategoryTypeViewModel(CategoryTypeViewModel categoryTypeViewModel)
        => ViewModel.LoadCategoryTypeViewModel(categoryTypeViewModel);

    // ReSharper disable once HeapView.ClosureAllocation
    public void SetTCategoryType(TCategoryType categoryType)
    {
        // categoryType.CopyPropertiesTo(CategoryType);
        // IsEditCategoryType = true;
        //
        // // ReSharper disable once HeapView.DelegateAllocation
        // // CategoryTypes.Remove(CategoryTypes.Find(s => s.Id.Equals(categoryType.Id))!);
    }

    // private static void ShowErrorMessage()
    //     => Dialogs.MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxValidateCategoryTypeErrorAlreadyExistMessage, MsgBoxImage.Warning);

    #endregion

    private void ButtonAddColor_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO correct
        // if (CategoryType.ColorFk is not null) EditColor();
        // else CreateNewColor();
    }

    private void EditColor()
    {
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // var addEditColorWindow = new AddEditColorWindow();
        // addEditColorWindow.SetTColor((int)CategoryType.ColorFk!);
        //
        // addEditColorWindow.ShowDialog();
        // if (addEditColorWindow.DialogResult is not true) return;
        // if (addEditColorWindow.DeleteColor)
        // {
        //     // ReSharper disable once HeapView.DelegateAllocation
        //     // var colorDeleted = Colors.FirstOrDefault(s => s.Id == CategoryType.ColorFk);
        //     // if (colorDeleted is not null) Colors.Remove(colorDeleted);
        //
        //     return;
        // }
        //
        // // ReSharper disable once HeapView.ClosureAllocation
        // var editedColor = addEditColorWindow.Color;
        //
        // Log.Information("Attempting to edit the color \"{AccountName}\"", editedColor.Name);
        // var (success, exception) = editedColor.AddOrEdit();
        // if (success)
        // {
        //     Log.Information("Color was successfully edited");
        //     var json = editedColor.ToJsonString();
        //     Log.Information("{Json}", json);
        //
        //     // ReSharper disable once HeapView.DelegateAllocation
        //     // var oldColor = Colors.First(s => s.Id.Equals(editedColor.Id));
        //     // editedColor.CopyPropertiesTo(oldColor);
        //
        //     Dialogs.MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxEditColorSuccessTitle,
        //         ColorManagementResources.MessageBoxEditColorSuccessMessage, MsgBoxImage.Check);
        // }
        // else
        // {
        //     Log.Error(exception, "An error occurred please retry");
        //     Dialogs.MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxEditColorErrorTitle,
        //         ColorManagementResources.MessageBoxEditColorErrorMessage, MsgBoxImage.Warning);
        // }
    }

    private void CreateNewColor()
    {
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // var addEditColorWindow = new AddEditColorWindow();
        // addEditColorWindow.ShowDialog();
        //
        // if (addEditColorWindow.DialogResult is not true) return;
        //
        // var newColor = addEditColorWindow.Color;
        //
        // Log.Information(
        //     "Attempt to inject the new color \"{ColorName}\" with hexadecimal code \"{ColorHexadecimalColorCode}\"",
        //     newColor.Name, newColor.HexadecimalColorCode);
        //
        // var (success, exception) = newColor.AddOrEdit();
        // if (success)
        // {
        //     Log.Information("color was successfully added");
        //     var json = newColor.ToJsonString();
        //     Log.Information("{Json}", json);
        //
        //     Dialogs.MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxAddColorSuccessMessage, MsgBoxImage.Check);
        //
        //     // Colors.AddAndSort(newColor, s => s.Name!);
        // }
        // else
        // {
        //     Log.Error(exception, "An error occurred please retry");
        //     Dialogs.MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxAddColorErrorTitle,
        //         ColorManagementResources.MessageBoxAddColorErrorMessage, MsgBoxImage.Error);
        // }
    }
}