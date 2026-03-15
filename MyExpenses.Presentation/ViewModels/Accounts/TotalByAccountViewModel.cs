using CommunityToolkit.Mvvm.ComponentModel;

namespace MyExpenses.Presentation.ViewModels.Accounts;

public partial class TotalByAccountViewModel : ObservableObject
{
    [ObservableProperty]
    public partial int Id { get; set; }

    [ObservableProperty]
    public required partial string Name { get; set; }

    [ObservableProperty]
    public partial double Total { get; set; }

    [ObservableProperty]
    public partial double TotalPointed { get; set; }

    [ObservableProperty]
    public partial double TotalNotPointed { get; set; }

    [ObservableProperty]
    public required partial string Symbol { get; set; }
}