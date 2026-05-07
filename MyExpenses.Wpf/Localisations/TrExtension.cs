using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MyExpenses.Wpf.Localisations;

/// <summary>
/// Represents a markup extension that provides localized content to the WPF UI
/// by binding a string resource key to localization managers.
/// </summary>
/// <remarks>
/// This extension is used in XAML to translate UI text based on a specified resource key.
/// It supports binding arguments for formatting the localized text.
///
/// Performance Optimization: Uses internal caching to reuse DummyProvider instances
/// with the same resource key, preventing multiple event subscriptions and reducing memory overhead.
/// </remarks>
/// <example>
/// Use this extension in XAML with a localization resource key, for example:
/// {loc:Tr MainWindowResources:WindowTitle}.
/// </example>
[MarkupExtensionReturnType(typeof(object))]
public class TrExtension : MarkupExtension
{
    // Cache to reuse DummyProvider instances and prevent memory leaks
    private static readonly Dictionary<string, DummyProvider> ProviderCache = [];

    public string? Value { get; set; }

    public System.Collections.ObjectModel.Collection<BindingBase>? Args { get; } = [];

    public TrExtension() { }

    public TrExtension(string value)
    {
        Value = value;
    }

    public TrExtension(object? value)
    {
        Value = value?.ToString();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrWhiteSpace(Value)) return "!INVALID_TR!";

        var parts = Value.Split(':', 2);
        if (parts.Length != 2) return $"!{Value}!";

        var managerName = parts[0];
        var key = parts[1];

        // Reuse cached provider to prevent creating multiple event subscriptions for the same key
        var cacheKey = $"{managerName}:{key}";
        if (!ProviderCache.TryGetValue(cacheKey, out var provider))
        {
            provider = new DummyProvider(managerName, key);
            ProviderCache[cacheKey] = provider;
        }

        if (Args is null || Args.Count is 0)
        {
            return new Binding(nameof(DummyProvider.Text))
            {
                Source = provider,
                Mode = BindingMode.OneWay
            }.ProvideValue(serviceProvider);
        }

        var multiBinding = new MultiBinding
        {
            Converter = new FormatConverter(),
            Mode = BindingMode.OneWay
        };

        multiBinding.Bindings.Add(new Binding(nameof(DummyProvider.Text))
        {
            Source = provider
        });

        foreach (var arg in Args)
        {
            multiBinding.Bindings.Add(arg);
        }

        return multiBinding.ProvideValue(serviceProvider);
    }

    private class DummyProvider : INotifyPropertyChanged, IDisposable
    {
        private readonly string _resourceManagerName;
        private readonly string _key;
        private EventHandler? _languageChangedHandler;

        /// <summary>
        /// Creates a localization provider that listens to language changes.
        /// Implements IDisposable to prevent memory leaks by unsubscribing from LocalizationService.LanguageChanged.
        /// </summary>
        public DummyProvider(string resourceManagerName, string key)
        {
            _resourceManagerName = resourceManagerName;
            _key = key;

            _languageChangedHandler = (_, _) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LanguageTag)));
            };

            LocalizationService.Instance.LanguageChanged += _languageChangedHandler;
        }

        public string Text
        {
            get
            {
                if (LocalizationResources.Managers.TryGetValue(_resourceManagerName, out var manager))
                {
                    var value = manager.GetString(_key, LocalizationService.Instance.CurrentCulture);
                    return value ?? $"!{_resourceManagerName}:{_key}!";
                }

                return $"!{_resourceManagerName}:{_key}!";
            }
        }

        public string LanguageTag
            => LocalizationService.Instance.CurrentCulture.IetfLanguageTag;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Dispose()
        {
            if (_languageChangedHandler != null)
            {
                LocalizationService.Instance.LanguageChanged -= _languageChangedHandler;
                _languageChangedHandler = null;
            }
            GC.SuppressFinalize(this);
        }

        ~DummyProvider()
        {
            Dispose();
        }
    }

    private class FormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length is 0) return "";

            var format = values[0]?.ToString();
            if (string.IsNullOrEmpty(format)) return "";

            var args = values.Skip(1).ToArray();

            try
            {
                return string.Format(format, args);
            }
            catch
            {
                return format;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}