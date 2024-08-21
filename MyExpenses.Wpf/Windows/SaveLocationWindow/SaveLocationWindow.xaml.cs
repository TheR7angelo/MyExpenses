using System.Windows;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows.SaveLocationWindow;

public partial class SaveLocationWindow
{
    #region DependencyProperty

    public static readonly DependencyProperty ButtonDropboxVisibilityProperty =
        DependencyProperty.Register(nameof(ButtonDropboxVisibility), typeof(bool), typeof(SaveLocationWindow),
            new PropertyMetadata(default(bool)));

    public bool ButtonDropboxVisibility
    {
        get => (bool)GetValue(ButtonDropboxVisibilityProperty);
        set => SetValue(ButtonDropboxVisibilityProperty, value);
    }

    public static readonly DependencyProperty ButtonLocalVisibilityProperty =
        DependencyProperty.Register(nameof(ButtonLocalVisibility), typeof(bool), typeof(SaveLocationWindow),
            new PropertyMetadata(default(bool)));

    public bool ButtonLocalVisibility
    {
        get => (bool)GetValue(ButtonLocalVisibilityProperty);
        set => SetValue(ButtonLocalVisibilityProperty, value);
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
}