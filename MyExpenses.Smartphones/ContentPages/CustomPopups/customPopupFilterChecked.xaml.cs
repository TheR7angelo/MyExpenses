using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CustomPopups.CustomPopupFilterChecked;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupFilterChecked : ICustomPopupFilter<BoolIsChecked>
{
    public static readonly BindableProperty ButtonCloseTextProperty = BindableProperty.Create(nameof(ButtonCloseText),
        typeof(string), typeof(CustomPopupFilterChecked));

    public string ButtonCloseText
    {
        get => (string)GetValue(ButtonCloseTextProperty);
        set => SetValue(ButtonCloseTextProperty, value);
    }

    private List<BoolIsChecked> OriginalIsCheckeds { get; }
    public List<BoolIsChecked> IsCheckeds { get; }

    public CustomPopupFilterChecked(IEnumerable<BoolIsChecked> currentHistoryIsCheckeds,
        IReadOnlyCollection<BoolIsChecked>? historyIsCheckedsAlreadyChecked = null)
    {
        OriginalIsCheckeds = [..currentHistoryIsCheckeds];
        IsCheckeds = [..OriginalIsCheckeds];

        if (historyIsCheckedsAlreadyChecked is not null)
        {
            foreach (var historyIsCheckedAlreadyChecked in historyIsCheckedsAlreadyChecked.Where(s => s.IsChecked))
            {
                var isChecked = IsCheckeds.FirstOrDefault(s => s.BoolValue.Equals(historyIsCheckedAlreadyChecked.BoolValue));
                if (isChecked is null) continue;
                isChecked.IsChecked = historyIsCheckedAlreadyChecked.IsChecked;
            }
        }

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonClose_OnClicked(object? sender, EventArgs e)
        => Close();

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    public IEnumerable<BoolIsChecked> GetFilteredItemChecked()
        => IsCheckeds.Where(s => s.IsChecked);

    public int GetFilteredItemCheckedCount()
        => IsCheckeds.Count(s => s.IsChecked);

    public int GetFilteredItemCount()
        => IsCheckeds.Count;

    private void UpdateLanguage()
    {
        ButtonCloseText = CustomPopupFilterCheckedResources.ButtonCloseText;
    }

    #endregion
}