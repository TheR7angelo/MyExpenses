using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.Pages.ColorManagementPage;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class ColorManagementPage
{
    public ObservableCollection<TColor> Colors { get; }

    public ColorManagementPage()
    {
        using var context = new DataBaseContext();
        Colors = [..context.TColors.OrderBy(s => s.Name)];

        InitializeComponent();

        Colors.CollectionChanged += Colors_OnCollectionChanged;
    }

    #region Action

    private void ButtonAddColor_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditColorWindow = new AddEditColorWindow();
        addEditColorWindow.ShowDialog();

        if (addEditColorWindow.DialogResult != true) return;

        var newColor = addEditColorWindow.Color;

        Log.Information("Attempt to inject the new color \"{ColorName}\" with hexadecimal code \"{ColorHexadecimalColorCode}\"",
            newColor.Name, newColor.HexadecimalColorCode);

        var (success, exception) = newColor.AddOrEdit();
        if (success)
        {
            Log.Information("color was successfully added");
            MsgBox.Show(ColorManagementPageResources.MessageBoxAddColorSuccess, MsgBoxImage.Check);

            Colors.AddAndSort(newColor, s => s.Name!);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(ColorManagementPageResources.MessageBoxAddColorError, MsgBoxImage.Error);
        }
    }

    private void ButtonEditColor_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not TColor colorToEdit) return;

        var addEditColorWindow = new AddEditColorWindow();
        addEditColorWindow.SetTColor(colorToEdit);

        addEditColorWindow.ShowDialog();
        if (addEditColorWindow.DialogResult != true) return;
        if (addEditColorWindow.DeleteColor)
        {
            var colorDeleted = Colors.FirstOrDefault(s => s.Id == colorToEdit.Id);
            if (colorDeleted is not null) Colors.Remove(colorDeleted);

            return;
        }

        var editedColor = addEditColorWindow.Color;

        Log.Information("Attempting to edit the color \"{AccountName}\"", editedColor.Name);
        var (success, exception) = editedColor.AddOrEdit();
        if (success)
        {
            Log.Information("Color was successfully edited");

            var oldColor = Colors.First(s => s.Id == editedColor.Id);
            editedColor.CopyPropertiesTo(oldColor);

            MsgBox.Show(ColorManagementPageResources.MessageBoxEditColorSuccess, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(ColorManagementPageResources.MessageBoxEditColorError, MsgBoxImage.Warning);
        }
    }

    private void Colors_OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => UpdateItemsSize();


    private void ItemsControl_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateItemsSize();

    #endregion

    #region Function

    private void UpdateItemsSize()
    {
        var items = ItemsControl.Items;
        double maxWidth = 0;

        foreach (var item in items)
        {
            ItemsControl.UpdateLayout();
            if (ItemsControl.ItemContainerGenerator.ContainerFromItem(item) is not FrameworkElement container) continue;
            maxWidth = Math.Max(maxWidth, container.ActualWidth);
        }

        foreach (var item in items)
        {
            if (ItemsControl.ItemContainerGenerator.ContainerFromItem(item) is not ContentPresenter container) continue;
            var button = container.FindChild<Button>();
            if (button is null) continue;

            button.Width = maxWidth;
        }
    }

    #endregion
}