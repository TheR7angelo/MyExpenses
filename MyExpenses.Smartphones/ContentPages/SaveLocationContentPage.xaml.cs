using System.Windows.Input;
using MyExpenses.Models.Wpf.Save;

namespace MyExpenses.Smartphones.ContentPages;

public partial class SaveLocationContentPage
{
    
    private readonly TaskCompletionSource<bool> _taskCompletionSource;
    
    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;
    
    public ICommand BackCommand { get; }
    
    public SaveLocation? SaveLocationResult { get; private set; }
    
    public SaveLocationContentPage()
    {
        BackCommand = new Command(OnBackCommandPressed);

        _taskCompletionSource = new TaskCompletionSource<bool>();
        
        InitializeComponent();
    }
    
    private async void OnBackCommandPressed()
    {
        _taskCompletionSource.SetResult(false);
        await Navigation.PopAsync();
    }

    private async void ButtonImageViewCellphone_OnClicked(object? sender, EventArgs e)
    {
        SaveLocationResult = SaveLocation.Local;
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private async void ButtonImageViewDropbox_OnClicked(object? sender, EventArgs e)
    {
        SaveLocationResult = SaveLocation.Dropbox;
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }
}