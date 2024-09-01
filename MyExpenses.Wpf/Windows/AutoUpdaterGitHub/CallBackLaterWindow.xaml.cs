using System.Windows;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Wpf.AutoUpdaterGitHub;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows.AutoUpdaterGitHub;

public partial class CallBackLaterWindow
{
    #region DependencyProperty

    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(CallBackLaterWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    #endregion

    public List<CallBackLaterTime> CallBackLaterTimes { get; } =
    [
        CallBackLaterTime.After30Minutes,
        CallBackLaterTime.After12Hours,
        CallBackLaterTime.After1Days,
        CallBackLaterTime.After2Days,
        CallBackLaterTime.After4Days,
        CallBackLaterTime.After8Days,
        CallBackLaterTime.After10Days
    ];

    public CallBackLaterTime? SelectedCallBackLaterTime { get; set; } = CallBackLaterTime.After30Minutes;

    public CallBackLaterWindow()
    {
        UpdateLanguage();

        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;

        this.SetWindowCornerPreference();
    }

    #region Action

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    #endregion

    private void UpdateLanguage()
    {
        TitleWindow = "Rappelez-moi plus tard la mise à jour";
    }

    private void CallBackLaterWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Console.WriteLine(e.NewSize);
    }
}