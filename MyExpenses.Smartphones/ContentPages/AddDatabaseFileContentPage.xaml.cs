using System.Windows.Input;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.Smartphones.Resources.Resx.ContentPages;
using MyExpenses.Utils.Strings;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AddDatabaseFileContentPage
{
    public static readonly BindableProperty DatabaseFilenameProperty = BindableProperty.Create(nameof(DatabaseFilename),
        typeof(string), typeof(AddDatabaseFileContentPage), default(string));

    public string DatabaseFilename
    {
        get => (string)GetValue(DatabaseFilenameProperty);
        set => SetValue(DatabaseFilenameProperty, value);
    }

    private List<ExistingDatabase> ExistingDatabases { get; } = [];

    private readonly TaskCompletionSource<bool> _taskCompletionSource;

    public static readonly BindableProperty CustomEntryControlPlaceholderTextProperty =
        BindableProperty.Create(nameof(CustomEntryControlPlaceholderText), typeof(string),
            typeof(AddDatabaseFileContentPage), default(string));

    public string CustomEntryControlPlaceholderText
    {
        get => (string)GetValue(CustomEntryControlPlaceholderTextProperty);
        set => SetValue(CustomEntryControlPlaceholderTextProperty, value);
    }

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public ICommand BackCommand { get; }

    public AddDatabaseFileContentPage()
    {
        BackCommand = new Command(OnBackCommandPressed);

        _taskCompletionSource = new TaskCompletionSource<bool>();

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += InterfaceOnLanguageChanged;
    }

    private void InterfaceOnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        CustomEntryControlPlaceholderText = AddDatabaseFileContentPageResources.CustomEntryControlPlaceholderText;
    }

    private async void OnBackCommandPressed()
    {
        _taskCompletionSource.SetResult(false);
        await Navigation.PopAsync();
    }

    #region Function

    public void SetExistingDatabase(IEnumerable<ExistingDatabase> existingDatabases)
        => ExistingDatabases.AddRange(existingDatabases);

    #endregion

    private async void ButtonValid_OnClicked(object? sender, EventArgs e)
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

    private async void ButtonCancel_OnClicked(object? sender, EventArgs e)
    {
        _taskCompletionSource.SetResult(false);
        await Navigation.PopAsync();
    }
}