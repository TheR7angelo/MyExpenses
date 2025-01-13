using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.Wpf.Resources.Resx.UserControls.Settings.LanguageControl;

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

    // ReSharper disable once HeapView.BoxingAllocation
    public static readonly DependencyProperty Is24HoursProperty = DependencyProperty.Register(nameof(Is24Hours),
        typeof(bool), typeof(LanguageControl), new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool Is24Hours
    {
        get => (bool)GetValue(Is24HoursProperty);
        set => SetValue(Is24HoursProperty, value);
    }

    public ObservableCollection<CultureInfo> CultureInfos { get; } = [];
    private List<string> CultureInfoCodes { get; }

    public LanguageControl()
    {
        CultureInfoSelected = CultureInfo.CurrentUICulture;

        var localFilePathDataBaseModel = DbContextBackup.LocalFilePathDataBaseModel;
        using var context = new DataBaseContext(localFilePathDataBaseModel);
        CultureInfoCodes = [..context.TSupportedLanguages.Select(s => s.Code)];

        var configuration = Config.Configuration;
        Is24Hours = configuration.Interface.Clock.Is24Hours;

        Interface.LanguageChanged += Interface_OnLanguageChanged;
        UpdateLanguage();

        InitializeComponent();
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        ComboBoxLanguageSelectorHintAssist = LanguageControlResources.ComboBoxLanguageSelectorHintAssist;
        LabelIs24HFormat = LanguageControlResources.LabelIs24HFormat;

        if (CultureInfos.Count is 0)
        {
            var cultureInfos = CultureInfoCodes.Select(s => new CultureInfo(s));
            CultureInfos.AddRange(cultureInfos);
        }
        else
        {
            var selectedCultureInfoCode = CultureInfoSelected.Name;
            for (var i = 0; i < CultureInfoCodes.Count; i++)
            {
                CultureInfos[i] = new CultureInfo(CultureInfoCodes[i]);
            }

            CultureInfoSelected = new CultureInfo(selectedCultureInfoCode);
        }
    }
}