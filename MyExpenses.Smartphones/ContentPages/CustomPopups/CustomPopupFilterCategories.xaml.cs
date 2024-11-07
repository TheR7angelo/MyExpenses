using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Derivatives.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupFilterCategories
{
    public List<VCategoryDerive> VCategoryDerives { get; }

    public CustomPopupFilterCategories()
    {
        var mapper = Mapping.Mapper;

        using var context = new DataBaseContext();
        VCategoryDerives = [..context.VCategories.Select(s => mapper.Map<VCategoryDerive>(s))];

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