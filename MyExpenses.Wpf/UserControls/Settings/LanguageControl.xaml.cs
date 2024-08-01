using System.Globalization;
using System.Windows;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;

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

    public static readonly DependencyProperty Is24HoursProperty = DependencyProperty.Register(nameof(Is24Hours),
        typeof(bool), typeof(LanguageControl), new PropertyMetadata(default(bool)));

    public bool Is24Hours
    {
        get => (bool)GetValue(Is24HoursProperty);
        set => SetValue(Is24HoursProperty, value);
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

        var configuration = Config.Configuration;
        Is24Hours = configuration.Interface.Clock.Is24Hours;

        InitializeComponent();
    }
}