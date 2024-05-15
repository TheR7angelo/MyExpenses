using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Pages;

public partial class ColorManagementPage
{
    public ObservableCollection<TColor> Colors { get; }

    //TODO work
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
        //TODO work
    }

    private void Colors_OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateItemsSize();
    }

    private void ItemsControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateItemsSize();
    }

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