using System.Globalization;
using System.Windows;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Settings;

public partial class LanguageControl
{
    public static readonly DependencyProperty CultureInfoSelectedProperty =
        DependencyProperty.Register(nameof(CultureInfoSelected), typeof(CultureInfo), typeof(LanguageControl),
            new PropertyMetadata(default(CultureInfo)));

    public CultureInfo CultureInfoSelected
    {
        get => (CultureInfo)GetValue(CultureInfoSelectedProperty);
        set => SetValue(CultureInfoSelectedProperty, value);
    }

    public List<CultureInfo> CultureInfos { get; } = [];

    public LanguageControl()
    {
        CultureInfoSelected = CultureInfo.CurrentUICulture;

        var localFilePathDataBaseModel = DbContextBackup.LocalFilePathDataBaseModel;
        using var context = new DataBaseContext(localFilePathDataBaseModel);
        foreach (var supportedLanguage in context.TSupportedLanguages)
        {
            var cultureInfo = new CultureInfo(supportedLanguage.Code);
            CultureInfos.Add(cultureInfo);
        }

        // var z = new CultureInfo("pt-PT");
        // CultureInfos.Add(z);

        InitializeComponent();
    }
}