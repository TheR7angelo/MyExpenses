using CommunityToolkit.Mvvm.ComponentModel;

namespace MyExpenses.Application.Models.Accounts;

public partial class TotalByAccountDto : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private double _total;

    [ObservableProperty]
    private double _totalPointed;

    [ObservableProperty]
    private double _totalNotPointed;

    [ObservableProperty]
    private string _symbol = string.Empty;
}