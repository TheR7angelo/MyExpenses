using System.Windows.Input;
using MyExpenses.Models.Wpf.Save;

namespace MyExpenses.Smartphones.ContentPages.SaveLocation;

public partial class SaveLocationContentPage
{
    public static readonly BindableProperty ButtonLocalVisibilityProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(ButtonLocalVisibility), typeof(bool), typeof(SaveLocationContentPage), false);

    public bool ButtonLocalVisibility
    {
        get => (bool)GetValue(ButtonLocalVisibilityProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        init => SetValue(ButtonLocalVisibilityProperty, value);
    }

    public static readonly BindableProperty ButtonDropboxVisibilityProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(ButtonDropboxVisibility), typeof(bool), typeof(SaveLocationContentPage), false);

    public bool ButtonDropboxVisibility
    {
        get => (bool)GetValue(ButtonDropboxVisibilityProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        init => SetValue(ButtonDropboxVisibilityProperty, value);
    }

    public static readonly BindableProperty ButtonFolderVisibilityProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(ButtonFolderVisibility), typeof(bool), typeof(SaveLocationContentPage), false);

    public bool ButtonFolderVisibility
    {
        get => (bool)GetValue(ButtonFolderVisibilityProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        init => SetValue(ButtonFolderVisibilityProperty, value);
    }

    private readonly TaskCompletionSource<bool> _taskCompletionSource;

    public static readonly BindableProperty ButtonDatabaseVisibilityProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(ButtonDatabaseVisibility), typeof(bool), typeof(SaveLocationContentPage), false);

    public bool ButtonDatabaseVisibility
    {
        get => (bool)GetValue(ButtonDatabaseVisibilityProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        init => SetValue(ButtonDatabaseVisibilityProperty, value);
    }

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public ICommand BackCommand { get; }

    public Models.Wpf.Save.SaveLocation? SaveLocationResult { get; private set; }

    public SaveLocationContentPage(SaveLocationMode saveLocationMode)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of the Command to bind the OnBackCommandPressed logic to the UI event.
        // This creates a command that encapsulates the behavior to be executed when the user triggers the "Back" action.
        BackCommand = new Command(OnBackCommandPressed);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of TaskCompletionSource to manage asynchronous communication between the dialog
        // and the calling logic. This allows the dialog to return a result (true or false) to the caller once
        // the user interacts with the UI elements.
        _taskCompletionSource = new TaskCompletionSource<bool>();

        switch (saveLocationMode)
        {
            case SaveLocationMode.LocalDropbox:
                ButtonLocalVisibility = true;
                ButtonDropboxVisibility = true;
                break;

            case SaveLocationMode.FolderFolderCompressDatabase:
                ButtonFolderVisibility = true;
                // ButtonFolderCompressVisibility = true;
                ButtonDatabaseVisibility = true;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(saveLocationMode), saveLocationMode, null);
        }

        InitializeComponent();
    }

    private void OnBackCommandPressed()
    {
        Task.Run(async () =>
        {
            _taskCompletionSource.SetResult(false);
            await Navigation.PopAsync();
        });
    }

    private void ButtonImageViewCellphone_OnClicked(object? sender, EventArgs e)
    {
        Task.Run(async () =>
        {
            SaveLocationResult = Models.Wpf.Save.SaveLocation.Local;
            _taskCompletionSource.SetResult(true);
            await Navigation.PopAsync();
        });
    }

    private void ButtonImageViewDropbox_OnClicked(object? sender, EventArgs e)
    {
        Task.Run(async () =>
        {
            SaveLocationResult = Models.Wpf.Save.SaveLocation.Dropbox;
            _taskCompletionSource.SetResult(true);
            await Navigation.PopAsync();
        });
    }

    private void ButtonImageViewFolder_OnClicked(object? sender, EventArgs e)
    {
        Task.Run(async () =>
        {
            SaveLocationResult = Models.Wpf.Save.SaveLocation.Folder;
            _taskCompletionSource.SetResult(true);
            await Navigation.PopAsync();
        });
    }

    private void ButtonImageViewDatabase_OnClicked(object? sender, EventArgs e)
    {
        Task.Run(async () =>
        {
            SaveLocationResult = Models.Wpf.Save.SaveLocation.Database;
            _taskCompletionSource.SetResult(true);
            await Navigation.PopAsync();
        });
    }
}