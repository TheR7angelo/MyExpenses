using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages;

public partial class ColorManagementContentPage
{
    public ObservableCollection<TColor> Colors { get; } = [];

    public ColorManagementContentPage()
    {
        RefreshColors();

        InitializeComponent();
    }

    private void RefreshColors()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        Colors.Clear();
        Colors.AddRange(context.TColors.OrderBy(s => s.Name));
    }

    private void TapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => _ = HandleTapGestureRecognizer(sender);

    private async Task HandleTapGestureRecognizer(object? sender)
    {
        if (sender is not Border border) return;
        if (border.BindingContext is not TColor color) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // A new instance of CustomPopupEditCategory is explicitly created here to allow customization
        // of the popup's behavior with specific properties such as MaxLength and CanDelete. This allocation
        // is necessary as each invocation may require a uniquely configured popup based on the application's
        // state or user interactions. The usage of an object initializer ensures these properties are set
        // immediately, avoiding uninitialized or invalid states for the popup.
        var colorPickerPopup = new ColorPickerPopup(); // { MaxLenght = MaxLength, CanDelete = true };
        colorPickerPopup.SetColor(color);
        await this.ShowPopupAsync(colorPickerPopup);

        // var result = await colorPickerPopup.ResultDialog;
        // if (result is ECustomPopupEntryResult.Cancel) return;
        //
        // category.CategoryName = customPopupEditCategory.EntryText;
        // category.ColorFk = customPopupEditCategory.SelectedColor?.Id;
        //
        // await HandleVCategoryResult(category, result);
        // RefreshCategories();
    }
}