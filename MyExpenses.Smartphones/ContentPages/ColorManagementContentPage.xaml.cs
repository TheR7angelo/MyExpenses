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
    {
        if (sender is not Border border) return;
        if (border.BindingContext is not TColor color) return;

        _ = HandleAddEditColor(color);
    }

    private void ButtonAddColor_OnClick(object? sender, EventArgs e)
        => _ = HandleAddEditColor();

    private async Task HandleAddEditColor(TColor? color = null)
    {
        var colorPickerPopup = new ColorPickerPopup { EditColor = true };
        if (color is not null) colorPickerPopup.SetColor(color);
        await this.ShowPopupAsync(colorPickerPopup);

        var result = await colorPickerPopup.ResultDialog;
        if (result is ECustomPopupEntryResult.Cancel) return;

        var hexadecimal = colorPickerPopup.BackgroundColor.ToArgbHex(true);
        var newColor = new TColor { Name = colorPickerPopup.ColorName, HexadecimalColorCode = hexadecimal };

        await HandleColorResult(result, newColor, color);

    }

    private async Task HandleColorResult(ECustomPopupEntryResult result, TColor newColor, TColor? oldColor)
    {
        //TODO work
    }
}