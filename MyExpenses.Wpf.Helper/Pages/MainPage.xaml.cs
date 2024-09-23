using System.Windows;
using MyExpenses.Wpf.Helper.Pages.SvgToXml;

namespace MyExpenses.Wpf.Helper.Pages;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void ButtonSvgToXml_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(SvgToXmlPage));
}