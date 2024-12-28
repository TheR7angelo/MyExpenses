using System.Windows.Input;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AddDatabaseFileContentPage;
using MyExpenses.Utils.Strings;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AddDatabaseFileContentPage
{
    public static readonly BindableProperty DatabaseFilenameProperty = BindableProperty.Create(nameof(DatabaseFilename),
        typeof(string), typeof(AddDatabaseFileContentPage));

    public string DatabaseFilename
    {
        get => (string)GetValue(DatabaseFilenameProperty);
        set => SetValue(DatabaseFilenameProperty, value);
    }

    private List<ExistingDatabase> ExistingDatabases { get; } = [];

    public static readonly BindableProperty ButtonCancelContentProperty =
        BindableProperty.Create(nameof(ButtonCancelContent), typeof(string), typeof(AddDatabaseFileContentPage));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    public static readonly BindableProperty ButtonValidContentProperty =
        BindableProperty.Create(nameof(ButtonValidContent), typeof(string), typeof(AddDatabaseFileContentPage));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    public static readonly BindableProperty CustomEntryControlPlaceholderTextProperty =
        BindableProperty.Create(nameof(CustomEntryControlPlaceholderText), typeof(string),
            typeof(AddDatabaseFileContentPage));

    public string CustomEntryControlPlaceholderText
    {
        get => (string)GetValue(CustomEntryControlPlaceholderTextProperty);
        set => SetValue(CustomEntryControlPlaceholderTextProperty, value);
    }

    private readonly TaskCompletionSource<bool> _taskCompletionSource;

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public ICommand BackCommand { get; }

    public AddDatabaseFileContentPage()
    {
        BackCommand = new Command(OnBackCommandPressed);

        _taskCompletionSource = new TaskCompletionSource<bool>();

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonCancel_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonCancel();

    private void ButtonValid_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonValid();

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void OnBackCommandPressed()
        => _ = HandleBackCommand();

    #endregion

    #region Function

    private async Task HandleBackCommand()
    {
        _taskCompletionSource.SetResult(false);
        await Navigation.PopAsync();
    }

    private async Task HandleButtonCancel()
    {
        _taskCompletionSource.SetResult(false);
        await Navigation.PopAsync();
    }

    private async Task HandleButtonValid()
    {
        if (string.IsNullOrEmpty(DatabaseFilename))
        {
            await DisplayAlert(AddDatabaseFileContentPageResources.MessageBoxEmptyNameErrorTitle,
                AddDatabaseFileContentPageResources.MessageBoxEmptyNameErrorMessage,
                AddDatabaseFileContentPageResources.MessageBoxEmptyNameErrorOkButton);
            return;
        }

        var containsIncorrectChar = DatabaseFilename.CheckFilenameContainsIncorrectChar();
        if (containsIncorrectChar)
        {
            await DisplayAlert(
                AddDatabaseFileContentPageResources.MessageBoxDatabaseFilenameContainsIncorrectCharErrorTitle,
                AddDatabaseFileContentPageResources.MessageBoxDatabaseFilenameContainsIncorrectCharErrorMessage,
                AddDatabaseFileContentPageResources.MessageBoxDatabaseFilenameContainsIncorrectCharErrorOkButton);
            return;
        }

        var alreadyExist = ExistingDatabases.Select(s => s.FileNameWithoutExtension).Contains(DatabaseFilename);
        if (alreadyExist)
        {
            await DisplayAlert(
                AddDatabaseFileContentPageResources.MessageBoxDatabaseFilenameContainsIncorrectCharErrorTitle,
                AddDatabaseFileContentPageResources.MessageBoxDatabaseFilenameContainsIncorrectCharErrorMessage,
                AddDatabaseFileContentPageResources.MessageBoxDatabaseFilenameContainsIncorrectCharErrorOkButton);
        }
        else
        {
            _taskCompletionSource.SetResult(true);
            await Navigation.PopAsync();
        }
    }

    public void SetExistingDatabase(IEnumerable<ExistingDatabase> existingDatabases)
        => ExistingDatabases.AddRange(existingDatabases);

    private void UpdateLanguage()
    {
        CustomEntryControlPlaceholderText = AddDatabaseFileContentPageResources.CustomEntryControlPlaceholderText;

        ButtonValidContent = AddDatabaseFileContentPageResources.ButtonValidContent;
        ButtonCancelContent = AddDatabaseFileContentPageResources.ButtonCancelContent;
    }

    #endregion
}