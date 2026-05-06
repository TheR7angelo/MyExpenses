using CommunityToolkit.Mvvm.ComponentModel;

namespace MyExpenses.Presentation.ViewModel;

public abstract class ViewModelBase : ObservableObject
{
    public bool IsBusy { get; set; }
    public string? ErrorMessage { get; set; }
}