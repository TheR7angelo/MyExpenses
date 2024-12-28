using System.Collections.ObjectModel;
using System.Windows.Input;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.SelectDatabaseFileContentPage;

namespace MyExpenses.Smartphones.ContentPages;

public partial class SelectDatabaseFileContentPage
{
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];
    public List<ExistingDatabase> ExistingDatabasesSelected { get; } = [];

    private readonly TaskCompletionSource<bool> _taskCompletionSource;

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

    public ICommand BackCommand { get; }

    public SelectDatabaseFileContentPage()
    {
        BackCommand = new Command(OnBackCommandPressed);

        _taskCompletionSource = new TaskCompletionSource<bool>();

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonCancel_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonCancel();

    private void ButtonValid_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonValid();

    private void ListView_OnItemTapped(object? sender, ItemTappedEventArgs e)
    {
        var selectedDatabase = e.Item as ExistingDatabase;
        if (ListView.TemplatedItems.FirstOrDefault(item => item.BindingContext == selectedDatabase) is not ViewCell
            viewCell) return;

        if (viewCell.View is not UraniumUI.Material.Controls.CheckBox checkBox) return;

        checkBox.IsChecked = !checkBox.IsChecked;
    }

    private void OnBackCommandPressed()
        => _ = HandleButtonCancel();

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
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
            if (viewCell.View is not UraniumUI.Material.Controls.CheckBox checkBox) continue;

            if (!checkBox.IsChecked) continue;
            if (viewCell.BindingContext is not ExistingDatabase existingDatabase) continue;

            ExistingDatabasesSelected.Add(existingDatabase);
        }

        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private void UpdateLanguage()
    {
        ButtonCancelContent = SelectDatabaseFileContentPageResources.ButtonCancelContent;
        ButtonValidMidContent = SelectDatabaseFileContentPageResources.ButtonValidMidContent;
    }

    #endregion
}