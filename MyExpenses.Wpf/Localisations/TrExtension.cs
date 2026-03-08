using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Markup;

namespace MyExpenses.Wpf.Localisations
{
    /// <summary>
    /// A markup extension that provides localized translated values for UI elements.
    /// This extension supports binding to resources in the format 'ResourceManagerName:Key'
    /// where 'ResourceManagerName' specifies the resource manager and 'Key' specifies
    /// the resource key to retrieve a localized string.
    /// </summary>
    public class TrExtension : MarkupExtension
    {
        public string ResourceManagerName { get; }
        public string Key { get; }

        public TrExtension(string resourceAndKey)
        {
            if (string.IsNullOrWhiteSpace(resourceAndKey))
                throw new ArgumentNullException(nameof(resourceAndKey));

            var parts = resourceAndKey.Split(':', 2);
            if (parts.Length != 2) throw new ArgumentException("TrExtension must be used in the format 'ResourceManagerName:Key'");

            ResourceManagerName = parts[0];
            Key = parts[1];
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding(nameof(DummyProvider.Text))
            {
                Source = new DummyProvider(ResourceManagerName, Key),
                Mode = BindingMode.OneWay
            };
            return binding.ProvideValue(serviceProvider);
        }

        private class DummyProvider : INotifyPropertyChanged
        {
            private readonly string _resourceManagerName;
            private readonly string _key;

            public DummyProvider(string resourceManagerName, string key)
            {
                _resourceManagerName = resourceManagerName;
                _key = key;
                LocalizationService.Instance.LanguageChanged += (_, _) =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }

            public string Text
            {
                get
                {
                    if (LocalizationResources.Managers.TryGetValue(_resourceManagerName, out var manager))
                        return manager.GetString(_key, LocalizationService.Instance.CurrentCulture)
                               ?? $"!{_resourceManagerName}:{_key}!";
                    return $"!{_resourceManagerName}:{_key}!";
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }
    }
}