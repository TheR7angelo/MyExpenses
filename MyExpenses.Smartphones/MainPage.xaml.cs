using System.Reflection;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones;

public partial class MainPage
{
    private int _count;

    public static readonly BindableProperty LabelContentProperty =
        BindableProperty.Create(nameof(LabelContent), typeof(string), typeof(MainPage), default(string));

    public string LabelContent
    {
        get => (string)GetValue(LabelContentProperty);
        set => SetValue(LabelContentProperty, value);
    }

    public MainPage()
    {
        // var dbFilePath = DbContextBackup.LocalFilePathDataBaseModel;
        // dbFilePath = Path.Join(FileSystem.AppDataDirectory, dbFilePath);
        var y = FileSystem.Current.OpenAppPackageFileAsync("Database Models\\Model.sqlite")
        .ConfigureAwait(false).GetAwaiter().GetResult();
        // var dbFilePath = Path.Join(FileSystem.AppDataDirectory, "Model.sqlite");
        // var z = File.Exists(dbFilePath);
        // using var context = new DataBaseContext(dbFilePath);
        //
        // LabelContent = context.TVersions.First().Version!.ToString();

        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames();

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