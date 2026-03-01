using CommunityToolkit.Mvvm.ComponentModel;

namespace MyExpenses.Application.ViewModels.Accounts;

public partial class TotalByAccountViewModel : ObservableObject
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