using System.Windows;
using MessageResult = MyExpenses.Presentation.Enums.MessageBoxResult;

namespace MyExpenses.Wpf.Windows.Dialogs.InputDialog;

public partial class InputDialogWindow
{
    public static readonly DependencyProperty TextBoxMaxLengthProperty =
        DependencyProperty.Register(nameof(TextBoxMaxLength), typeof(int), typeof(InputDialogWindow),
            new PropertyMetadata(0));

    public int TextBoxMaxLength
    {
        get => (int)GetValue(TextBoxMaxLengthProperty);
        init => SetValue(TextBoxMaxLengthProperty, value);
    }

    public static readonly DependencyProperty TextBoxTextProperty = DependencyProperty.Register(nameof(TextBoxText),
        typeof(string), typeof(InputDialogWindow), new PropertyMetadata(default(string)));

    public string TextBoxText
    {
        get => (string)GetValue(TextBoxTextProperty);
        set => SetValue(TextBoxTextProperty, value);
    }

    public MessageResult MessageBoxResult { get; private set; } = MessageResult.None;

    public InputDialogWindow()
    {
        InitializeComponent();
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
        => SetResult(MessageResult.Valid);

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        => SetResult(MessageResult.Cancel);

    private void SetResult(MessageResult result)
    {
        MessageBoxResult = result;
        DialogResult = true;
        Close();
    }
}