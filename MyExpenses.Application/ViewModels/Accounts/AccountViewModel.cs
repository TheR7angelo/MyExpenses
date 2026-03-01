using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MyExpenses.Application.ViewModels.Accounts;

public partial class AccountViewModel : ObservableValidator
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    [property: Required(ErrorMessage = "Account name is required")]
    [property: MaxLength(50, ErrorMessage = "Account name cannot exceed 50 characters")]
    private string _name = null!;

    [ObservableProperty]
    [property: Required(ErrorMessage = "Account type is required")]
    private int _accountTypeFk;

    [ObservableProperty]
    [property: Required(ErrorMessage = "Currency is required")]
    private int _currencyFk;

    [ObservableProperty]
    [property: Required(ErrorMessage = "Active status is required")]
    private bool _active;

    [ObservableProperty]
    [property: Required(ErrorMessage = "Date added is required")]
    private DateTime _dateAdded;
}