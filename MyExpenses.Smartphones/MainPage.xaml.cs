namespace MyExpenses.Smartphones;

public partial class MainPage
{
    private int _count;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        _count++;

        CounterBtn.Text = _count is 1
            ? $"Clicked {_count} time"
            : $"Clicked {_count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}