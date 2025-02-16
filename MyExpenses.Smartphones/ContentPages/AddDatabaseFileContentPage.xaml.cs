using System.Windows.Input;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.SharedUtils.Resources.Resx.AddDatabaseFile;
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

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // TaskCompletionSource is intentionally allocated here as it is the fundamental mechanism
    // for creating and controlling the completion of the Task exposed by `ResultDialog`.
    // This object is required to manually signal task completion (`SetResult`, `SetException`, etc.)
    // when the operation is resolved, ensuring proper asynchronous flow.
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public ICommand BackCommand { get; }

    public AddDatabaseFileContentPage()
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

    private void ButtonCancel_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonCancel();

    private void ButtonValid_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonValid();

    private void Interface_OnLanguageChanged()
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
            await DisplayAlert(AddDatabaseFileResources.MessageBoxEmptyNameErrorTitle,
                AddDatabaseFileResources.MessageBoxEmptyNameErrorMessage,
                AddDatabaseFileResources.MessageBoxEmptyNameErrorOkButton);
            return;
        }

        var containsIncorrectChar = DatabaseFilename.CheckFilenameContainsIncorrectChar();
        if (containsIncorrectChar)
        {
            await DisplayAlert(
                AddDatabaseFileResources.MessageBoxDatabaseFilenameContainsIncorrectCharErrorTitle,
                AddDatabaseFileResources.MessageBoxDatabaseFilenameContainsIncorrectCharErrorMessage,
                AddDatabaseFileResources.MessageBoxDatabaseFilenameContainsIncorrectCharErrorOkButton);
            return;
        }

        var alreadyExist = ExistingDatabases.Select(s => s.FileNameWithoutExtension).Contains(DatabaseFilename);
        if (alreadyExist)
        {
            await DisplayAlert(
                AddDatabaseFileResources.MessageBoxDatabaseAlreadyExistErrorTitle,
                AddDatabaseFileResources.MessageBoxDatabaseAlreadyExistErrorMessage,
                AddDatabaseFileResources.MessageBoxDatabaseAlreadyExistErrorOkButton);
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
        CustomEntryControlPlaceholderText = AddDatabaseFileResources.TextBoxHintAssist;
        ButtonValidContent = AddDatabaseFileResources.ButtonValidContent;
        ButtonCancelContent = AddDatabaseFileResources.ButtonCancelContent;
    }

    #endregion
}