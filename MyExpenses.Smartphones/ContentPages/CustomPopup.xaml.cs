namespace MyExpenses.Smartphones.ContentPages;

public partial class CustomPopup
{
    public CustomPopup()
    {
        InitializeComponent();
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        Close();
    }
}