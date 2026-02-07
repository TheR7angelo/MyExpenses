using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources;
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

    public LanguageControl()
    {
        CultureInfoSelected = CultureInfo.CurrentUICulture;
        CultureInfos.AddRange(LanguagesUtils.GetSupportedCultures());

        var configuration = Config.Configuration;
        Is24Hours = configuration.Interface.Clock.Is24Hours;

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

        var selectedName = CultureInfoSelected.Name;

        CultureInfos.Clear();
        CultureInfos.AddRange(LanguagesUtils.GetSupportedCultures());

        CultureInfoSelected = CultureInfos.FirstOrDefault(c => c.Name == selectedName) ?? CultureInfo.CurrentUICulture;
    }
}