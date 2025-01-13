using System.Windows;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows.SaveLocationWindow;

public partial class SaveLocationWindow
{
    #region DependencyProperty

    // ReSharper disable once HeapView.BoxingAllocation
    public static readonly DependencyProperty ButtonFolderCompressVisibilityProperty =
        DependencyProperty.Register(nameof(ButtonFolderCompressVisibility), typeof(bool), typeof(SaveLocationWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool ButtonFolderCompressVisibility
    {
        get => (bool)GetValue(ButtonFolderCompressVisibilityProperty);
        set => SetValue(ButtonFolderCompressVisibilityProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    public static readonly DependencyProperty ButtonDatabaseVisibilityProperty =
        DependencyProperty.Register(nameof(ButtonDatabaseVisibility), typeof(bool), typeof(SaveLocationWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool ButtonDatabaseVisibility
    {
        get => (bool)GetValue(ButtonDatabaseVisibilityProperty);
        init => SetValue(ButtonDatabaseVisibilityProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    public static readonly DependencyProperty ButtonFolderVisibilityProperty =
        DependencyProperty.Register(nameof(ButtonFolderVisibility), typeof(bool), typeof(SaveLocationWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool ButtonFolderVisibility
    {
        get => (bool)GetValue(ButtonFolderVisibilityProperty);
        init => SetValue(ButtonFolderVisibilityProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    public static readonly DependencyProperty ButtonDropboxVisibilityProperty =
        DependencyProperty.Register(nameof(ButtonDropboxVisibility), typeof(bool), typeof(SaveLocationWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool ButtonDropboxVisibility
    {
        get => (bool)GetValue(ButtonDropboxVisibilityProperty);
        init => SetValue(ButtonDropboxVisibilityProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    public static readonly DependencyProperty ButtonLocalVisibilityProperty =
        DependencyProperty.Register(nameof(ButtonLocalVisibility), typeof(bool), typeof(SaveLocationWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool ButtonLocalVisibility
    {
        get => (bool)GetValue(ButtonLocalVisibilityProperty);
        init => SetValue(ButtonLocalVisibilityProperty, value);
    }

    #endregion

    public SaveLocation? SaveLocationResult { get; private set; }

    public SaveLocationWindow(SaveLocationMode saveLocationMode)
    {
        switch (saveLocationMode)
        {
            case SaveLocationMode.LocalDropbox:
                ButtonLocalVisibility = true;
                ButtonDropboxVisibility = true;
                break;

            case SaveLocationMode.FolderFolderCompressDatabase:
                ButtonFolderVisibility = true;
                ButtonFolderCompressVisibility = true;
                ButtonDatabaseVisibility = true;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(saveLocationMode), saveLocationMode, null);
        }

        InitializeComponent();

        this.SetWindowCornerPreference();
    }

    private void ButtonDropbox_OnClick(object sender, RoutedEventArgs e)
    {
        SaveLocationResult = SaveLocation.Dropbox;
        DialogResult = true;
        Close();
    }

    private void ButtonLocal_OnClick(object sender, RoutedEventArgs e)
    {
        SaveLocationResult = SaveLocation.Local;
        DialogResult = true;
        Close();
    }

    private void ButtonFolder_OnClick(object sender, RoutedEventArgs e)
    {
        SaveLocationResult = SaveLocation.Folder;
        DialogResult = true;
        Close();
    }

    private void ButtonDatabase_OnClick(object sender, RoutedEventArgs e)
    {
        SaveLocationResult = SaveLocation.Database;
        DialogResult = true;
        Close();
    }
}