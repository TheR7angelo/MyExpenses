using System.Globalization;
using System.Windows;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;

namespace MyExpenses.Wpf.UserControls.Settings;

public partial class LanguageControl
{
    #region Resx

    public static readonly DependencyProperty ComboBoxLanguageSelectorHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxLanguageSelectorHintAssist), typeof(string), typeof(LanguageControl),
            new PropertyMetadata(default(string)));

    public string ComboBoxLanguageSelectorHintAssist
    {
        get => (string)GetValue(ComboBoxLanguageSelectorHintAssistProperty);
        set => SetValue(ComboBoxLanguageSelectorHintAssistProperty, value);
    }

    public static readonly DependencyProperty LabelIs24HFormatProperty =
        DependencyProperty.Register(nameof(LabelIs24HFormat), typeof(string), typeof(LanguageControl),
            new PropertyMetadata(default(string)));

    public string LabelIs24HFormat
    {
        get => (string)GetValue(LabelIs24HFormatProperty);
        set => SetValue(LabelIs24HFormatProperty, value);
    }

    #endregion

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

        Interface.LanguageChanged += Interface_OnLanguageChanged;
        UpdateLanguage();

        InitializeComponent();
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        // TODO work
        ComboBoxLanguageSelectorHintAssist = "Choice of application language";
        LabelIs24HFormat = "Is 24h format";
    }
}