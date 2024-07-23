using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.Utils.WindowStyle;
using MyExpenses.Wpf.Resources.Resx.Windows.AddDatabaseFileWindow;
using MyExpenses.Wpf.Windows.MsgBox;

namespace MyExpenses.Wpf.Windows;

public partial class AddDatabaseFileWindow
{
    public static readonly DependencyProperty DatabaseFilenameProperty =
        DependencyProperty.Register(nameof(DatabaseFilename), typeof(string), typeof(AddDatabaseFileWindow),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow), typeof(string), typeof(AddDatabaseFileWindow), new PropertyMetadata(default(string)));

    public string DatabaseFilename
    {
        get => (string)GetValue(DatabaseFilenameProperty);
        set => SetValue(DatabaseFilenameProperty, value);
    }

    public string TextBoxHintAssist { get; } = AddDatabaseFileWindowResources.TextBoxHintAssist;
    public string ButtonValidContent { get; } = AddDatabaseFileWindowResources.ButtonValidContent;
    public string ButtonCancelContent { get; } = AddDatabaseFileWindowResources.ButtonCancelContent;

    private List<ExistingDatabase> ExistingDatabases { get; } = [];

    public object TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    public AddDatabaseFileWindow()
    {
        Interface.LanguageChanged += Interface_OnLanguageChanged;
        UpdateLanguage();
        InitializeComponent();

        var hWnd = new WindowInteropHelper(GetWindow(this)!).EnsureHandle();
        hWnd.SetWindowCornerPreference(DwmWindowCornerPreference.Round);
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        TitleWindow = AddDatabaseFileWindowResources.TitleWindow;
    }

    #region Action

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(DatabaseFilename))
        {
            MsgBox.MsgBox.Show(AddDatabaseFileWindowResources.MessageBoxEmptyNameError, MsgBoxImage.Error);
            return;
        }

        var containsIncorrectChar = CheckDatabaseFilenameIncorrectChar(DatabaseFilename);
        if (containsIncorrectChar) ShowErrorMessageContainsIncorrectChar();

        var alreadyExist = CheckDatabaseFilename(DatabaseFilename);
        if (alreadyExist) ShowErrorMessageAlreadyExist();
        else
        {
            DialogResult = true;
            Close();
        }
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void TextBoxDatabaseFilename_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var databaseFilename = textBox.Text;
        if (string.IsNullOrEmpty(databaseFilename)) return;

        var containsIncorrectChar = CheckDatabaseFilenameIncorrectChar(databaseFilename);
        if (containsIncorrectChar) ShowErrorMessageContainsIncorrectChar();

        var alreadyExist = CheckDatabaseFilename(databaseFilename);
        if (alreadyExist) ShowErrorMessageAlreadyExist();
    }


    #endregion

    #region Function

    private bool CheckDatabaseFilename(string databaseFilename)
        => ExistingDatabases.Select(s => s.FileNameWithoutExtension).Contains(databaseFilename);

    private bool CheckDatabaseFilenameIncorrectChar(string databaseFilename)
    {
        if (databaseFilename.StartsWith('.')) return true;
        var charsIncorrects = new[] {'/', '\\', '?', '%', '*', ':', '|', '"', '<', '>', '\0'};
        return charsIncorrects.Any(databaseFilename.Contains);
    }

    public void SetExistingDatabase(IEnumerable<ExistingDatabase> existingDatabases)
        => ExistingDatabases.AddRange(existingDatabases);


    private void ShowErrorMessageAlreadyExist()
        => MsgBox.MsgBox.Show(AddDatabaseFileWindowResources.MessageBoxDatabaseAlreadyExistError, MsgBoxImage.Warning);

    private void ShowErrorMessageContainsIncorrectChar()
        => MsgBox.MsgBox.Show(AddDatabaseFileWindowResources.MessageBoxDatabaseFilenameContainsIncorrectCharError, MsgBoxImage.Error);

    #endregion
}