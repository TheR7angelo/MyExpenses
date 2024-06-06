using System.Windows;
using System.Windows.Input;
using MyExpenses.Wpf.Resources.Resx.Windows.MainWindow;

namespace MyExpenses.Wpf;

public partial class MainWindow
{
    public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register(nameof(CanGoBack),
        typeof(bool), typeof(MainWindow), new PropertyMetadata(default(bool)));

    #region MenuItemFile

    public string MenuItemHeaderFile { get; } = MainWindowResources.MenuItemHeaderFile;

    #region MenuItem Database

    public string MenuItemHeaderDatabase { get; } = MainWindowResources.MenuItemHeaderDatabase;

    public string MenuItemHeaderExportDatabase { get; } = MainWindowResources.MenuItemHeaderExportDatabase;
    public string MenuItemHeaderImportDatabase { get; } = MainWindowResources.MenuItemHeaderImportDatabase;

    #endregion

    public string MenuItemHeaderPrevious { get; } = MainWindowResources.MenuItemHeaderPrevious;
    public string MenuItemHeaderSettings { get; } = MainWindowResources.MenuItemHeaderSettings;

    public bool CanGoBack
    {
        get => (bool)GetValue(CanGoBackProperty);
        set => SetValue(CanGoBackProperty, value);
    }

    #endregion

    public MainWindow()
    {
        // var blackList = new List<KnownColor>
        // {
        //     KnownColor.Control, KnownColor.Desktop, KnownColor.Highlight, KnownColor.Info, KnownColor.Menu,
        //     KnownColor.Transparent, KnownColor.Window, KnownColor.ActiveBorder, KnownColor.ActiveCaption,
        //     KnownColor.AppWorkspace, KnownColor.ButtonFace, KnownColor.ButtonHighlight, KnownColor.ButtonShadow,
        //     KnownColor.ControlDark, KnownColor.ControlLight, KnownColor.ControlText, KnownColor.GrayText,
        //     KnownColor.HighlightText, KnownColor.HotTrack, KnownColor.InactiveBorder, KnownColor.InactiveCaption,
        //     KnownColor.InactiveCaptionText, KnownColor.InfoText, KnownColor.MenuBar, KnownColor.MenuHighlight,
        //     KnownColor.MenuText, KnownColor.ScrollBar, KnownColor.WindowFrame, KnownColor.WindowText,
        //     KnownColor.ActiveCaptionText, KnownColor.ControlDarkDark, KnownColor.ControlLightLight,
        //     KnownColor.GradientActiveCaption, KnownColor.GradientInactiveCaption
        // };
        // var knownColors = Enum.GetValues<KnownColor>()
        //     .Where(s => !blackList.Contains(s))
        //     .OrderBy(s => s.ToString())
        //     .ToList();
        //
        // var colors = new List<TColor>();
        // foreach (var knownColor in knownColors)
        // {
        //     var name = knownColor.ToString().SplitUpperCaseWord();
        //     var color = Color.FromKnownColor(knownColor);
        //     var hex = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        //
        //     colors.Add(new TColor
        //     {
        //         Name = name,
        //         HexadecimalColorCode = hex
        //     });
        // }
        //
        // using var context = new DataBaseContext();
        // context.TColors.AddRange(colors);
        // context.SaveChanges();

        InitializeComponent();

        Navigator.CanGoBackChanged += Navigator_OnCanGoBackChanged;
    }

    private void Navigator_OnCanGoBackChanged(object? sender, NavigatorEventArgs e)
    {
        CanGoBack = e.CanGoBack;
    }

    private void MenuItemPrevious_OnClick(object sender, RoutedEventArgs e)
        => nameof(FrameBody).GoBack();

    private void FrameBody_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.XButton1) nameof(FrameBody).GoBack();
        else if (e.ChangedButton == MouseButton.XButton2) nameof(FrameBody).GoForward();
    }
}