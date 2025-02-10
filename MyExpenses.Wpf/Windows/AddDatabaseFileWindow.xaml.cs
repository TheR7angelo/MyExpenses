using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.Utils.Strings;
using MyExpenses.Wpf.Resources.Resx.Windows.AddDatabaseFileWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;

namespace MyExpenses.Wpf.Windows;

public partial class AddDatabaseFileWindow
{
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxHintAssist), typeof(string), typeof(AddDatabaseFileWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxHintAssist
    {
        get => (string)GetValue(TextBoxHintAssistProperty);
        set => SetValue(TextBoxHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(AddDatabaseFileWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonValidContentProperty =
        DependencyProperty.Register(nameof(ButtonValidContent), typeof(string), typeof(AddDatabaseFileWindow),
            new PropertyMetadata(default(string)));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty DatabaseFilenameProperty =
        DependencyProperty.Register(nameof(DatabaseFilename), typeof(string), typeof(AddDatabaseFileWindow),
            new PropertyMetadata(default(string)));

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(AddDatabaseFileWindow), new PropertyMetadata(default(string)));

    public string DatabaseFilename
    {
        get => (string)GetValue(DatabaseFilenameProperty);
        set => SetValue(DatabaseFilenameProperty, value);
    }

    private List<ExistingDatabase> ExistingDatabases { get; } = [];

    public object TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    public AddDatabaseFileWindow()
    {
        UpdateLanguage();

        InitializeComponent();

        this.SetWindowCornerPreference();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(DatabaseFilename))
        {
            MsgBox.MsgBox.Show(AddDatabaseFileWindowResources.MessageBoxEmptyNameError, MsgBoxImage.Error);
            return;
        }

        var containsIncorrectChar = DatabaseFilename.CheckFilenameContainsIncorrectChar();
        if (containsIncorrectChar)
        {
            ShowErrorMessageContainsIncorrectChar();
            return;
        }

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

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void TextBoxDatabaseFilename_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var databaseFilename = textBox.Text;
        if (string.IsNullOrEmpty(databaseFilename)) return;

        var containsIncorrectChar = databaseFilename.CheckFilenameContainsIncorrectChar();
        if (containsIncorrectChar) ShowErrorMessageContainsIncorrectChar();

        var alreadyExist = CheckDatabaseFilename(databaseFilename);
        if (alreadyExist) ShowErrorMessageAlreadyExist();
    }

    #endregion

    #region Function

    private bool CheckDatabaseFilename(string databaseFilename)
        => ExistingDatabases.Select(s => s.FileNameWithoutExtension).Contains(databaseFilename);

    public void SetExistingDatabase(IEnumerable<ExistingDatabase> existingDatabases)
        => ExistingDatabases.AddRange(existingDatabases);


    private static void ShowErrorMessageAlreadyExist()
        => MsgBox.MsgBox.Show(AddDatabaseFileWindowResources.MessageBoxDatabaseAlreadyExistError, MsgBoxImage.Warning);

    private static void ShowErrorMessageContainsIncorrectChar()
        => MsgBox.MsgBox.Show(AddDatabaseFileWindowResources.MessageBoxDatabaseFilenameContainsIncorrectCharError,
            MsgBoxImage.Error);

    private void UpdateLanguage()
    {
        TitleWindow = AddDatabaseFileWindowResources.TitleWindow;

        TextBoxHintAssist = AddDatabaseFileWindowResources.TextBoxHintAssist;
        ButtonValidContent = AddDatabaseFileWindowResources.ButtonValidContent;
        ButtonCancelContent = AddDatabaseFileWindowResources.ButtonCancelContent;
    }

    #endregion
}