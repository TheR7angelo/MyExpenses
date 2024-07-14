using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.Utils.WindowStyle;
using MyExpenses.Wpf.Resources.Resx.Windows.SelectDatabaseFileWindow;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class SelectDatabaseFileWindow
{
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];

    public List<ExistingDatabase> ExistingDatabasesSelected { get; } = [];

    public static readonly DependencyProperty SelectDatabaseFileWindowTitleProperty =
        DependencyProperty.Register(nameof(SelectDatabaseFileWindowTitle), typeof(string),
            typeof(SelectDatabaseFileWindow), new PropertyMetadata(default(string)));

    public string SelectDatabaseFileWindowTitle
    {
        get => (string)GetValue(SelectDatabaseFileWindowTitleProperty);
        set => SetValue(SelectDatabaseFileWindowTitleProperty, value);
    }

    public static readonly DependencyProperty ButtonValidContentProperty =
        DependencyProperty.Register(nameof(ButtonValidContent), typeof(string), typeof(SelectDatabaseFileWindow),
            new PropertyMetadata(default(string)));

    public object ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(SelectDatabaseFileWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    public SelectDatabaseFileWindow()
    {
        Interface.LanguageChanged += Interface_OnLanguageChanged;
        UpdateStringLanguage();

        InitializeComponent();

        var hWnd = new WindowInteropHelper(GetWindow(this)!).EnsureHandle();
        hWnd.SetWindowCornerPreference(DwmWindowCornerPreference.Round);
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateStringLanguage();

    private void UpdateStringLanguage()
    {
        SelectDatabaseFileWindowTitle = SelectDatabaseFileWindowResources.SelectDatabaseFileWindowTitle;

        ButtonCancelContent = SelectDatabaseFileWindowResources.ButtonCancelCotent;
        ButtonValidContent = SelectDatabaseFileWindowResources.ButtonValidContent;
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var checkBoxesChecked = ListView.FindVisualChildren<CheckBox>()
            .Where(s => (bool)s.IsChecked!).ToList();

        foreach (var existingDatabase in checkBoxesChecked
                     .Select(checkBoxChecked => checkBoxChecked.DataContext as ExistingDatabase)
                     .OfType<ExistingDatabase>())
        {
            ExistingDatabasesSelected.Add(existingDatabase);
        }

        DialogResult = true;
        Close();
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}