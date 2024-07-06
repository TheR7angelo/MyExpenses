using System.Windows;
using System.Windows.Interop;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.Utils.WindowStyle;

namespace MyExpenses.Wpf.Windows.SaveLocationWindow;

public partial class SaveLocationWindow
{
    public SaveLocation SaveLocationResult { get; private set; } = SaveLocation.Local;

    public SaveLocationWindow()
    {
        InitializeComponent();

        var hWnd = new WindowInteropHelper(GetWindow(this)!).EnsureHandle();
        hWnd.SetWindowCornerPreference(DwmWindowCornerPreference.Round);
    }

    private void ButtonDropbox_OnClick(object sender, RoutedEventArgs e)
    {
        SaveLocationResult = SaveLocation.Dropbox;
        DialogResult = true;
        Close();
    }

    private void ButtonLocal_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}