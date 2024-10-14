using System.Collections.ObjectModel;
using System.Windows.Input;
using MyExpenses.Models.IO;

namespace MyExpenses.Smartphones.ContentPages;

public partial class SelectDatabaseFileContentPage
{
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];
    public List<ExistingDatabase> ExistingDatabasesSelected { get; } = [];

    private readonly TaskCompletionSource<bool> _taskCompletionSource;
    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public ICommand BackCommand { get; }

    public SelectDatabaseFileContentPage()
    {
        BackCommand = new Command(OnBackCommandPressed);

        _taskCompletionSource = new TaskCompletionSource<bool>();

        InitializeComponent();
    }

    #region Action

    private void ListView_OnItemTapped(object? sender, ItemTappedEventArgs e)
    {
        var selectedDatabase = e.Item as ExistingDatabase;
        if (ListView.TemplatedItems.FirstOrDefault(item => item.BindingContext == selectedDatabase) is not ViewCell viewCell) return;

        if (viewCell.View is not HorizontalStackLayout horizontalStackLayout) return;

        if (horizontalStackLayout.Children.FirstOrDefault(s => s.GetType() == typeof(CheckBox)) is not CheckBox checkBox) return;

        checkBox.IsChecked = !checkBox.IsChecked;
    }

    private async void OnBackCommandPressed()
    {
        _taskCompletionSource.SetResult(false);
        await Navigation.PopAsync();
    }

    #endregion

    private void ButtonValid_OnClick(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ButtonCancel_OnClick(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}