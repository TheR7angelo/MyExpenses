using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupFilterCategories
{
    public List<VCategory> VCategories { get; }

    public CustomPopupFilterCategories()
    {
        using var context = new DataBaseContext();
        VCategories = [..context.VCategories];

        InitializeComponent();
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        Close();
    }

    private void SearchBar_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        //TODO work
    }
}