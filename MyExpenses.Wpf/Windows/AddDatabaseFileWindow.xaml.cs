using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.IO;
using MyExpenses.Wpf.Windows.MsgBox;

namespace MyExpenses.Wpf.Windows;

public partial class AddDatabaseFileWindow
{
    public static readonly DependencyProperty DatabaseFilenameProperty =
        DependencyProperty.Register(nameof(DatabaseFilename), typeof(string), typeof(AddDatabaseFileWindow),
            new PropertyMetadata(default(string)));

    public string DatabaseFilename
    {
        get => (string)GetValue(DatabaseFilenameProperty);
        set => SetValue(DatabaseFilenameProperty, value);
    }

    //TODO work
    public string TextBoxHintAssist { get; } = "Database filename :";
    //TODO work
    public string ButtonValidContent { get; } = "Valid";
    //TODO work
    public string ButtonCancelContent { get; } = "Cancel";

    private List<ExistingDatabase> ExistingDatabases { get; } = [];

    public AddDatabaseFileWindow()
    {
        InitializeComponent();
    }

    #region Action

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(DatabaseFilename))
        {
            //TODO work
            MsgBox.MsgBox.Show("Database filename cannot be empty", MsgBoxImage.Error);
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
        var charsIncorrects = new[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
        return charsIncorrects.Any(databaseFilename.Contains);
    }

    public void SetExistingDatabase(IEnumerable<ExistingDatabase> existingDatabases)
        => ExistingDatabases.AddRange(existingDatabases);


    //TODO work
    private void ShowErrorMessageAlreadyExist()
        => MsgBox.MsgBox.Show("Database filename already exist", MsgBoxImage.Warning);

    //TODO work
    private void ShowErrorMessageContainsIncorrectChar()
        => MsgBox.MsgBox.Show("Database filename contains incorrect characters", MsgBoxImage.Error);

    #endregion
}