using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.Pages.Test;

public partial class ButtonsStylesPage
{
    private int _count;

    public static readonly BindableProperty LabelContentProperty =
        BindableProperty.Create(nameof(LabelContent), typeof(string), typeof(ButtonsStylesPage), default(string));

    public string LabelContent
    {
        get => (string)GetValue(LabelContentProperty);
        set => SetValue(LabelContentProperty, value);
    }

    public static readonly BindableProperty LabelCountContentProperty =
        BindableProperty.Create(nameof(LabelCountContent), typeof(string), typeof(ButtonsStylesPage), "Clicked 0 time");

    public string LabelCountContent
    {
        get => (string)GetValue(LabelCountContentProperty);
        set => SetValue(LabelCountContentProperty, value);
    }

    public ButtonsStylesPage()
    {
        var dbFilePath = DbContextBackup.LocalFilePathDataBaseModel;
        using var context = new DataBaseContext(dbFilePath);

        LabelContent = context.TVersions.First().Version!.ToString();

        InitializeComponent();
    }

    #region Action

    private void OnCounterClicked(object sender, EventArgs e)
    {
        _count++;

        LabelCountContent = _count is 1
            ? $"Clicked {_count} time"
            : $"Clicked {_count} times";

        SemanticScreenReader.Announce(LabelCountContent);
    }

    #endregion
}