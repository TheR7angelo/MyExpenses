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

    private List<ExistingDatabase> ExistingDatabases { get; } = [];

    public string DatabaseFilename
    {
        get => (string)GetValue(DatabaseFilenameProperty);
        set => SetValue(DatabaseFilenameProperty, value);
    }

    public AddDatabaseFileWindow()
    {
        InitializeComponent();
    }

    #region Action

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(DatabaseFilename))
        {
            MsgBox.MsgBox.Show("Database filename cannot be empty", MsgBoxImage.Error);
            return;
        }

        var alreadyExist = CheckDatabaseFilename(DatabaseFilename);
        if (alreadyExist) ShowErrorMessage();
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

        var alreadyExist = CheckDatabaseFilename(databaseFilename);
        if (alreadyExist) ShowErrorMessage();
    }

    #endregion

    #region Function

    private bool CheckDatabaseFilename(string databaseFilename)
        => ExistingDatabases.Select(s => s.FileNameWithoutExtension).Contains(databaseFilename);

    public void SetExistingDatabase(IEnumerable<ExistingDatabase> existingDatabases)
        => ExistingDatabases.AddRange(existingDatabases);


    private void ShowErrorMessage()
        => MsgBox.MsgBox.Show("Database filename already exist", MsgBoxImage.Warning);

    #endregion
}