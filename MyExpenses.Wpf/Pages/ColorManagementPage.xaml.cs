using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Sql;
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
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        Colors = [..context.TColors.OrderBy(s => s.Name)];

        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Colors.CollectionChanged += Colors_OnCollectionChanged;
    }

    #region Action

    private void ButtonAddColor_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The instance of AddEditColorWindow is created locally within this method and is used temporarily.
        // Since there are no references to it after this scope and the Garbage Collector will handle
        // its cleanup efficiently, this allocation is intentional and does not require further optimization.
        var addEditColorWindow = new AddEditColorWindow();
        addEditColorWindow.ShowDialog();

        if (addEditColorWindow.DialogResult is not true) return;

        var newColor = addEditColorWindow.Color;

        Log.Information("Attempt to inject the new color \"{ColorName}\" with hexadecimal code \"{ColorHexadecimalColorCode}\"",
            newColor.Name, newColor.HexadecimalColorCode);

        var (success, exception) = newColor.AddOrEdit();
        if (success)
        {
            Log.Information("color was successfully added");
            var json = newColor.ToJsonString();
            Log.Information("{Json}", json);

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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The instance of AddEditColorWindow is created locally within this method and is used temporarily.
        // Since there are no references to it after this scope and the Garbage Collector will handle
        // its cleanup efficiently, this allocation is intentional and does not require further optimization.
        var addEditColorWindow = new AddEditColorWindow();
        addEditColorWindow.SetTColor(colorToEdit);

        addEditColorWindow.ShowDialog();
        if (addEditColorWindow.DialogResult is not true) return;
        if (addEditColorWindow.DeleteColor)
        {
            // ReSharper disable once HeapView.DelegateAllocation
            var colorDeleted = Colors.FirstOrDefault(s => s.Id.Equals(colorToEdit.Id));
            if (colorDeleted is not null) Colors.Remove(colorDeleted);

            return;
        }

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