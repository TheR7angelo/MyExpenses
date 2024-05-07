using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Pages;

public partial class CategoryTypeManagementPage
{
    public ObservableCollection<TCategoryType> CategoryTypes { get; }

    public required DashBoardPage DashBoardPage { get; init; }

    public CategoryTypeManagementPage()
    {
        using var context = new DataBaseContext();
        CategoryTypes = [..context.TCategoryTypes];

        InitializeComponent();
    }
}