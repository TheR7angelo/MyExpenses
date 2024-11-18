using System.Windows.Input;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AccountTypeSummaryContentPage
{
    public ICommand BackCommand { get; set; }

    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;
    
    public AccountTypeSummaryContentPage()
    {
        BackCommand = new Command(OnBackCommandPressed);
        
        InitializeComponent();
    }

    private async void OnBackCommandPressed()
    {
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }
}