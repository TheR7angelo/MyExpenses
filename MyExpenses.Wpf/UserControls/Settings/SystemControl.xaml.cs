using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.SharedUtils.Utils;
using MyExpenses.Utils;
using MyExpenses.Utils.Strings;
using MyExpenses.Wpf.Resources.Resx.UserControls.Settings.SystemControl;

namespace MyExpenses.Wpf.UserControls.Settings;

public partial class SystemControl
{
    public static readonly DependencyProperty ButtonOpenBackupDirectoryContentProperty =
        DependencyProperty.Register(nameof(ButtonOpenBackupDirectoryContent), typeof(string), typeof(SystemControl),
            new PropertyMetadata(default(string)));

    public string ButtonOpenBackupDirectoryContent
    {
        get => (string)GetValue(ButtonOpenBackupDirectoryContentProperty);
        set => SetValue(ButtonOpenBackupDirectoryContentProperty, value);
    }

    public static readonly DependencyProperty ButtonOpenLogDirectoryContentProperty =
        DependencyProperty.Register(nameof(ButtonOpenLogDirectoryContent), typeof(string), typeof(SystemControl),
            new PropertyMetadata(default(string)));

    public string ButtonOpenLogDirectoryContent
    {
        get => (string)GetValue(ButtonOpenLogDirectoryContentProperty);
        set => SetValue(ButtonOpenLogDirectoryContentProperty, value);
    }

    public static readonly DependencyProperty MaxBackupDatabaseStringProperty =
        DependencyProperty.Register(nameof(MaxBackupDatabaseString), typeof(string), typeof(SystemControl),
            new PropertyMetadata(default(string)));

    public string MaxBackupDatabaseString
    {
        get => (string)GetValue(MaxBackupDatabaseStringProperty);
        set => SetValue(MaxBackupDatabaseStringProperty, value);
    }

    public static readonly DependencyProperty MaxBackupDatabaseProperty =
        DependencyProperty.Register(nameof(MaxBackupDatabase), typeof(int), typeof(SystemControl),
            new PropertyMetadata(0));

    public int MaxBackupDatabase
    {
        get => (int)GetValue(MaxBackupDatabaseProperty);
        set => SetValue(MaxBackupDatabaseProperty, value);
    }

    public static readonly DependencyProperty MaxDaysLogProperty = DependencyProperty.Register(nameof(MaxDaysLog),
        typeof(int), typeof(SystemControl), new PropertyMetadata(0));

    public int MaxDaysLog
    {
        get => (int)GetValue(MaxDaysLogProperty);
        set => SetValue(MaxDaysLogProperty, value);
    }

    public static readonly DependencyProperty MaxDaysLogStringProperty = DependencyProperty.Register(nameof(MaxDaysLogString),
        typeof(string), typeof(SystemControl), new PropertyMetadata(default(string)));

    public string MaxDaysLogString
    {
        get => (string)GetValue(MaxDaysLogStringProperty);
        set => SetValue(MaxDaysLogStringProperty, value);
    }

    public SystemControl()
    {
        MaxDaysLog = Config.Configuration.System.MaxDaysLog;
        MaxBackupDatabase = Config.Configuration.System.MaxBackupDatabase;

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += InterfaceOnLanguageChanged;
    }

    private void InterfaceOnLanguageChanged()
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        ButtonOpenLogDirectoryContent = SystemControlResources.ButtonOpenLogDirectoryContent;
        ButtonOpenBackupDirectoryContent = SystemControlResources.ButtonOpenBackupDirectoryContent;

        MaxDaysLogString = SystemControlResources.MaxDaysLogString;
        MaxBackupDatabaseString = SystemControlResources.MaxBackupDatabaseString;
    }

    private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var numericUpDown = (NumericUpDown)sender;
        var txt = numericUpDown.Value + e.Text;

        var canConvert = txt.ToDouble(out _);

        e.Handled = !canConvert;
    }

    private void ButtonOpenLogDirectory_OnClick(object sender, RoutedEventArgs e)
        => OsInfos.LogDirectoryPath.StartFile();

    private void ButtonOpenBackupDirectory_OnClick(object sender, RoutedEventArgs e)
        => DatabaseInfos.LocalDirectoryBackupDatabase.StartFile();
}