using System.Windows;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Wpf.AutoUpdaterGitHub;
using MyExpenses.Wpf.Resources.Resx.Windows.CallBackLaterWindow;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows.AutoUpdaterGitHub;

public partial class CallBackLaterWindow
{
    #region DependencyProperty

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(CallBackLaterWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBlockDownloadLaterQuestionProperty =
        DependencyProperty.Register(nameof(TextBlockDownloadLaterQuestion), typeof(string), typeof(CallBackLaterWindow),
            new PropertyMetadata(default(string)));

    public string TextBlockDownloadLaterQuestion
    {
        get => (string)GetValue(TextBlockDownloadLaterQuestionProperty);
        set => SetValue(TextBlockDownloadLaterQuestionProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBlockDownloadLaterContentProperty =
        DependencyProperty.Register(nameof(TextBlockDownloadLaterContent), typeof(string), typeof(CallBackLaterWindow),
            new PropertyMetadata(default(string)));

    public string TextBlockDownloadLaterContent
    {
        get => (string)GetValue(TextBlockDownloadLaterContentProperty);
        set => SetValue(TextBlockDownloadLaterContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty RadioButtonDownloadLaterYesProperty =
        DependencyProperty.Register(nameof(RadioButtonDownloadLaterYes), typeof(string), typeof(CallBackLaterWindow),
            new PropertyMetadata(default(string)));

    public string RadioButtonDownloadLaterYes
    {
        get => (string)GetValue(RadioButtonDownloadLaterYesProperty);
        set => SetValue(RadioButtonDownloadLaterYesProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty RadioButtonDownloadLaterNoProperty =
        DependencyProperty.Register(nameof(RadioButtonDownloadLaterNo), typeof(string), typeof(CallBackLaterWindow),
            new PropertyMetadata(default(string)));

    public string RadioButtonDownloadLaterNo
    {
        get => (string)GetValue(RadioButtonDownloadLaterNoProperty);
        set => SetValue(RadioButtonDownloadLaterNoProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonOkContentProperty =
        DependencyProperty.Register(nameof(ButtonOkContent), typeof(string), typeof(CallBackLaterWindow),
            new PropertyMetadata(default(string)));

    public string ButtonOkContent
    {
        get => (string)GetValue(ButtonOkContentProperty);
        set => SetValue(ButtonOkContentProperty, value);
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

    public CallBackLaterTime SelectedCallBackLaterTime { get; set; } = CallBackLaterTime.After30Minutes;
    public bool RadioButtonDownloadLaterYesIsChecked { get; set; } = true;
    public bool RadioButtonDownloadLaterNoIsChecked { get; set; }

    public CallBackLaterWindow()
    {
        UpdateLanguage();

        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;

        this.SetWindowCornerPreference();
    }

    #region Action

    private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    private void UpdateLanguage()
    {
        TitleWindow = CallBackLaterWindowResources.TitleWindow;

        TextBlockDownloadLaterQuestion = CallBackLaterWindowResources.TextBlockDownloadLaterQuestion;
        TextBlockDownloadLaterContent = CallBackLaterWindowResources.TextBlockDownloadLaterContent;
        RadioButtonDownloadLaterYes = CallBackLaterWindowResources.RadioButtonDownloadLaterYes;
        RadioButtonDownloadLaterNo = CallBackLaterWindowResources.RadioButtonDownloadLaterNo;
        ButtonOkContent = CallBackLaterWindowResources.ButtonOkContent;
    }

    #endregion
}