using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.SharedUtils.Objects;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.UserControls.Settings.LanguageControl;

namespace MyExpenses.Wpf.UserControls.Settings;

public partial class LanguageControl
{
    #region Resx

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ComboBoxLanguageSelectorHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxLanguageSelectorHintAssist), typeof(string), typeof(LanguageControl),
            new PropertyMetadata(default(string)));

    public string ComboBoxLanguageSelectorHintAssist
    {
        get => (string)GetValue(ComboBoxLanguageSelectorHintAssistProperty);
        set => SetValue(ComboBoxLanguageSelectorHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LabelIs24HFormatProperty =
        DependencyProperty.Register(nameof(LabelIs24HFormat), typeof(string), typeof(LanguageControl),
            new PropertyMetadata(default(string)));

    public string LabelIs24HFormat
    {
        get => (string)GetValue(LabelIs24HFormatProperty);
        set => SetValue(LabelIs24HFormatProperty, value);
    }

    #endregion

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty CultureInfoSelectedProperty =
        DependencyProperty.Register(nameof(CultureInfoSelected), typeof(CultureInfo), typeof(LanguageControl),
            new PropertyMetadata(default(CultureInfo)));

    public CultureInfo CultureInfoSelected
    {
        get => (CultureInfo)GetValue(CultureInfoSelectedProperty);
        set => SetValue(CultureInfoSelectedProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
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

        // Creating a new DataBaseContext instance is required to access the database.
        // This usage is expected and unavoidable as each call represents a discrete transactional context.
        // The "using" statement ensures proper disposal of the context after use.
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        using var context = new DataBaseContext(DatabaseInfos.LocalFilePathDataBaseModel);
        CultureInfoCodes = [..context.TSupportedLanguages.Select(s => s.Code)];

        var configuration = Config.Configuration;
        Is24Hours = configuration.Interface.Clock.Is24Hours;

        UpdateLanguage();

        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        ComboBoxLanguageSelectorHintAssist = LanguageControlResources.ComboBoxLanguageSelectorHintAssist;
        LabelIs24HFormat = LanguageControlResources.LabelIs24HFormat;

        if (CultureInfos.Count is 0)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var cultureInfos = CultureInfoCodes.Select(s => new CultureInfo(s));
            CultureInfos.AddRange(cultureInfos);
        }
        else
        {
            var originalSelectedCultureInfoCode = CultureInfoSelected.DeepCopy()!;
            for (var i = 0; i < CultureInfoCodes.Count; i++)
            {
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                CultureInfos[i] = new CultureInfo(CultureInfoCodes[i]);
            }

            CultureInfoSelected = originalSelectedCultureInfoCode;
        }
    }
}