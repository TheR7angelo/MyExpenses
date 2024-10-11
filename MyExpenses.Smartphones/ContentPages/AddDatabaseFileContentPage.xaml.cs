using System.Windows.Input;
using MyExpenses.Models.IO;
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
            //TODO trad
            await DisplayAlert("Error", "Database file name cannot be empty", "Ok");
            return;
        }

        var containsIncorrectChar = DatabaseFilename.CheckFilenameContainsIncorrectChar();
        if (containsIncorrectChar)
        {
            //TODO trad
            await DisplayAlert("Error", "Database file name contains incorrect characters", "Ok");
        }

        //TODO trad
        var alreadyExist = ExistingDatabases.Select(s => s.FileNameWithoutExtension).Contains(DatabaseFilename);
        if (alreadyExist) await DisplayAlert("Error", "Database file name already exist", "Ok");
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