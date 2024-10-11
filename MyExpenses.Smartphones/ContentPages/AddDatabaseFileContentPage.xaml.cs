using System.Windows.Input;
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

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public ICommand BackCommand { get; }

    public AddDatabaseFileContentPage()
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
        }

        var alreadyExist = ExistingDatabases.Select(s => s.FileNameWithoutExtension).Contains(DatabaseFilename);
        if (alreadyExist)
            await DisplayAlert(
                AddDatabaseFileContentPageResources.MessageBoxDatabaseFilenameContainsIncorrectCharErrorTitle,
                AddDatabaseFileContentPageResources.MessageBoxDatabaseFilenameContainsIncorrectCharErrorMessage,
                AddDatabaseFileContentPageResources.MessageBoxDatabaseFilenameContainsIncorrectCharErrorOkButton);
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