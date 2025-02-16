using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Derivatives.Tables;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CustomPopups.CustomPopupFilterPlaces;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupFilterPlaces : ICustomPopupFilter<TPlaceDerive>
{
    public static readonly BindableProperty ButtonCloseTextProperty = BindableProperty.Create(nameof(ButtonCloseText),
        typeof(string), typeof(CustomPopupFilterPlaces));

    public string ButtonCloseText
    {
        get => (string)GetValue(ButtonCloseTextProperty);
        set => SetValue(ButtonCloseTextProperty, value);
    }

    public static readonly BindableProperty SearchBarPlaceHolderTextProperty =
        BindableProperty.Create(nameof(SearchBarPlaceHolderText), typeof(string), typeof(CustomPopupFilterPlaces));

    public string SearchBarPlaceHolderText
    {
        get => (string)GetValue(SearchBarPlaceHolderTextProperty);
        set => SetValue(SearchBarPlaceHolderTextProperty, value);
    }

    public static readonly BindableProperty GeometrySourceProperty = BindableProperty.Create(nameof(GeometrySource),
        // ReSharper disable once HeapView.BoxingAllocation
        typeof(EPackIcons), typeof(CustomPopupFilterPlaces), EPackIcons.CheckboxBlankOutline);

    public EPackIcons GeometrySource
    {
        get => (EPackIcons)GetValue(GeometrySourceProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(GeometrySourceProperty, value);
    }

    private List<TPlaceDerive> OriginalPlaceDerives { get; }
    public List<TPlaceDerive> PlaceDerives { get; }

    private string? SearchText { get; set; }

    public CustomPopupFilterPlaces(IReadOnlyCollection<TPlaceDerive> currentTPlaceDerives,
        IReadOnlyCollection<TPlaceDerive>? modePlacesAlreadyChecked = null)
    {
        OriginalPlaceDerives = [..currentTPlaceDerives];
        PlaceDerives = [..OriginalPlaceDerives];

        if (modePlacesAlreadyChecked is not null)
        {
            // ReSharper disable once HeapView.ClosureAllocation
            foreach (var modePlaceAlreadyChecked in modePlacesAlreadyChecked.Where(s => s.IsChecked).ToArray())
            {
                // ReSharper disable once HeapView.DelegateAllocation
                var placeDerive = PlaceDerives.FirstOrDefault(s => s.Id.Equals(modePlaceAlreadyChecked.Id));
                if (placeDerive is null) continue;
                placeDerive.IsChecked = modePlaceAlreadyChecked.IsChecked;
            }
        }

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonClose_OnClicked(object? sender, EventArgs e)
        => Close();

    private void CheckBox_OnCheckedChanged(object? sender, EventArgs eventArgs)
        => CalculateCheckboxIconGeometrySource();

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void SearchBar_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        SearchText = e.NewTextValue;

        FilterPlaceBySearchText();
    }

    private void SvgPath_OnClicked(object? sender, EventArgs e)
    {
        var check = GeometrySource is EPackIcons.CheckboxBlankOutline;

        foreach (var originalPlaceDerive in OriginalPlaceDerives)
        {
            originalPlaceDerive.IsChecked = check;
        }

        FilterPlaceBySearchText();
        CalculateCheckboxIconGeometrySource();
    }

    #endregion

    #region Function

    private void CalculateCheckboxIconGeometrySource()
    {
        var allPlacesCount = OriginalPlaceDerives.Count;
        var placeDerivesCheckedCount = GetFilteredItemCheckedCount();

        EPackIcons icon;
        if (placeDerivesCheckedCount is 0) icon = EPackIcons.CheckboxBlankOutline;
        else if (placeDerivesCheckedCount.Equals(allPlacesCount)) icon = EPackIcons.CheckboxOutline;
        else icon = EPackIcons.MinusCheckboxOutline;

        GeometrySource = icon;
    }

    private void FilterPlaceBySearchText()
    {
        SearchText ??= string.Empty;

        // ReSharper disable once HeapView.DelegateAllocation
        var filterPlaces = OriginalPlaceDerives.Where(s => s.Name!.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase));

        PlaceDerives.Clear();
        PlaceDerives.AddRange(filterPlaces);
    }

    public IEnumerable<TPlaceDerive> GetFilteredItemChecked()
        => PlaceDerives.Where(s => s.IsChecked);

    public int GetFilteredItemCheckedCount()
        => PlaceDerives.Count(s => s.IsChecked);

    public int GetFilteredItemCount()
        => OriginalPlaceDerives.Count;


    private void UpdateLanguage()
    {
        SearchBarPlaceHolderText = CustomPopupFilterPlacesResources.SearchBarPlaceHolderText;
        ButtonCloseText = CustomPopupFilterPlacesResources.ButtonCloseText;
    }

    #endregion
}