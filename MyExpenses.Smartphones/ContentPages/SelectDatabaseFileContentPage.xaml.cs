using System.Collections.ObjectModel;
using System.Windows.Input;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.SharedUtils.Resources.Resx.SelectDatabaseFileManagement;

namespace MyExpenses.Smartphones.ContentPages;

public partial class SelectDatabaseFileContentPage
{
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];
    public List<ExistingDatabase> ExistingDatabasesSelected { get; } = [];

    public static readonly BindableProperty LabelStatusProperty = BindableProperty.Create(nameof(LabelStatus),
        typeof(string), typeof(SelectDatabaseFileContentPage));

    public string LabelStatus
    {
        get => (string)GetValue(LabelStatusProperty);
        set => SetValue(LabelStatusProperty, value);
    }

    public static readonly BindableProperty ButtonCancelContentProperty =
        BindableProperty.Create(nameof(ButtonCancelContent), typeof(string), typeof(SelectDatabaseFileContentPage));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    public static readonly BindableProperty ButtonValidMidContentProperty =
        BindableProperty.Create(nameof(ButtonValidMidContent), typeof(string), typeof(SelectDatabaseFileContentPage));

    public string ButtonValidMidContent
    {
        get => (string)GetValue(ButtonValidMidContentProperty);
        set => SetValue(ButtonValidMidContentProperty, value);
    }

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // TaskCompletionSource is intentionally allocated here as it is the fundamental mechanism
    // for creating and controlling the completion of the Task exposed by `ResultDialog`.
    // This object is required to manually signal task completion (`SetResult`, `SetException`, etc.)
    // when the operation is resolved, ensuring proper asynchronous flow.
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public ICommand BackCommand { get; }

    public SelectDatabaseFileContentPage()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // ReSharper disable once HeapView.DelegateAllocation
        // The Command object is explicitly created here to handle the user's interaction with the UI.
        // This allocation is necessary because `Command` encapsulates the behavior (in this case, `OnBackCommandPressed`)
        // and binds it to the associated UI element, such as a Button or a gesture.
        // This ensures proper separation between the UI and logic layers.
        BackCommand = new Command(OnBackCommandPressed);

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonCancel_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonCancel();

    private void ButtonValid_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonValid();

    private void ListView_OnItemTapped(object? sender, ItemTappedEventArgs e)
    {
        // ReSharper disable once HeapView.ClosureAllocation
        if (e.Item is not ExistingDatabase selectedDatabase) return;

        // ReSharper disable once HeapView.DelegateAllocation
        if (ListView.TemplatedItems.FirstOrDefault(item => item.BindingContext == selectedDatabase) is not ViewCell
            viewCell) return;

        if (viewCell.View is not UraniumUI.Material.Controls.CheckBox checkBox) return;

        checkBox.IsChecked = !checkBox.IsChecked;
    }

    private void OnBackCommandPressed()
        => _ = HandleButtonCancel();

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    private async Task HandleButtonCancel()
    {
        _taskCompletionSource.SetResult(false);
        await Navigation.PopAsync();
    }

    private async Task HandleButtonValid()
    {
        ExistingDatabasesSelected.Clear();

        var viewCells = ListView.TemplatedItems.Where(s => s.GetType() == typeof(ViewCell))
            .Select(s => (ViewCell)s);

        foreach (var viewCell in viewCells)
        {
            if (viewCell.View is not Grid grid) continue;

            var checkBox = grid.Children.OfType<UraniumUI.Material.Controls.CheckBox>().FirstOrDefault();
            if (checkBox is null) continue;

            if (!checkBox.IsChecked) continue;
            if (viewCell.BindingContext is not ExistingDatabase existingDatabase) continue;

            ExistingDatabasesSelected.Add(existingDatabase);
        }

        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private void UpdateLanguage()
    {
        LabelStatus = SelectDatabaseFileManagementResources.LabelStatusContent;
        ButtonCancelContent = SelectDatabaseFileManagementResources.ButtonCancelContent;
        ButtonValidMidContent = SelectDatabaseFileManagementResources.ButtonValidContent;
    }

    #endregion
}