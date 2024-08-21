using System.Windows;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows.SaveLocationWindow;

public partial class SaveLocationWindow
{
    public SaveLocation? SaveLocationResult { get; private set; }

    public SaveLocationWindow()
    {
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