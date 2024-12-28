using System.Windows.Input;
using MyExpenses.Models.Wpf.Save;

namespace MyExpenses.Smartphones.ContentPages.SaveLocation;

public partial class SaveLocationContentPage
{
    public static readonly BindableProperty ButtonLocalVisibilityProperty =
        BindableProperty.Create(nameof(ButtonLocalVisibility), typeof(bool), typeof(SaveLocationContentPage),
            false);

    public bool ButtonLocalVisibility
    {
        get => (bool)GetValue(ButtonLocalVisibilityProperty);
        set => SetValue(ButtonLocalVisibilityProperty, value);
    }

    public static readonly BindableProperty ButtonDropboxVisibilityProperty =
        BindableProperty.Create(nameof(ButtonDropboxVisibility), typeof(bool), typeof(SaveLocationContentPage),
            false);

    public bool ButtonDropboxVisibility
    {
        get => (bool)GetValue(ButtonDropboxVisibilityProperty);
        set => SetValue(ButtonDropboxVisibilityProperty, value);
    }

    public static readonly BindableProperty ButtonFolderVisibilityProperty =
        BindableProperty.Create(nameof(ButtonFolderVisibility), typeof(bool), typeof(SaveLocationContentPage),
            false);

    public bool ButtonFolderVisibility
    {
        get => (bool)GetValue(ButtonFolderVisibilityProperty);
        set => SetValue(ButtonFolderVisibilityProperty, value);
    }

    private readonly TaskCompletionSource<bool> _taskCompletionSource;

    public static readonly BindableProperty ButtonDatabaseVisibilityProperty =
        BindableProperty.Create(nameof(ButtonDatabaseVisibility), typeof(bool), typeof(SaveLocationContentPage),
            false);

    public bool ButtonDatabaseVisibility
    {
        get => (bool)GetValue(ButtonDatabaseVisibilityProperty);
        set => SetValue(ButtonDatabaseVisibilityProperty, value);
    }

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public ICommand BackCommand { get; }

    public Models.Wpf.Save.SaveLocation? SaveLocationResult { get; private set; }

    public SaveLocationContentPage(SaveLocationMode saveLocationMode)
    {
        BackCommand = new Command(OnBackCommandPressed);

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

    private async void OnBackCommandPressed()
    {
        _taskCompletionSource.SetResult(false);
        await Navigation.PopAsync();
    }

    private async void ButtonImageViewCellphone_OnClicked(object? sender, EventArgs e)
    {
        SaveLocationResult = Models.Wpf.Save.SaveLocation.Local;
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private async void ButtonImageViewDropbox_OnClicked(object? sender, EventArgs e)
    {
        SaveLocationResult = Models.Wpf.Save.SaveLocation.Dropbox;
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private async void ButtonImageViewFolder_OnClicked(object? sender, EventArgs e)
    {
        SaveLocationResult = Models.Wpf.Save.SaveLocation.Folder;
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private async void ButtonImageViewDatabase_OnClicked(object? sender, EventArgs e)
    {
        SaveLocationResult = Models.Wpf.Save.SaveLocation.Database;
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }
}