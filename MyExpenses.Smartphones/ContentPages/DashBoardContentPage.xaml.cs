using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MyExpenses.Maui.Utils;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopupFilter;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Derivatives.Views;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.DashBoardContentPage;
using MyExpenses.Smartphones.UserControls.Images;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Strings;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class DashBoardContentPage
{
    public static readonly BindableProperty LabelTextValueProperty = BindableProperty.Create(nameof(LabelTextValue),
        typeof(string), typeof(DashBoardContentPage), default(string));

    public string LabelTextValue
    {
        get => (string)GetValue(LabelTextValueProperty);
        set => SetValue(LabelTextValueProperty, value);
    }

    public static readonly BindableProperty LabelTextPaymentModeProperty =
        BindableProperty.Create(nameof(LabelTextPaymentMode), typeof(string), typeof(DashBoardContentPage),
            default(string));

    public string LabelTextPaymentMode
    {
        get => (string)GetValue(LabelTextPaymentModeProperty);
        set => SetValue(LabelTextPaymentModeProperty, value);
    }

    public static readonly BindableProperty LabelTextDescriptionProperty =
        BindableProperty.Create(nameof(LabelTextDescription), typeof(string), typeof(DashBoardContentPage),
            default(string));

    public string LabelTextDescription
    {
        get => (string)GetValue(LabelTextDescriptionProperty);
        set => SetValue(LabelTextDescriptionProperty, value);
    }

    public static readonly BindableProperty LabelTextCategoryProperty =
        BindableProperty.Create(nameof(LabelTextCategory), typeof(string), typeof(DashBoardContentPage),
            default(string));

    public string LabelTextCategory
    {
        get => (string)GetValue(LabelTextCategoryProperty);
        set => SetValue(LabelTextCategoryProperty, value);
    }

    public static readonly BindableProperty ComboBoxMonthHintAssistProperty =
        BindableProperty.Create(nameof(ComboBoxMonthHintAssist), typeof(string), typeof(DashBoardContentPage),
            default(string));

    public string ComboBoxMonthHintAssist
    {
        get => (string)GetValue(ComboBoxMonthHintAssistProperty);
        set => SetValue(ComboBoxMonthHintAssistProperty, value);
    }

    public static readonly BindableProperty ComboBoxYearsHintAssistProperty =
        BindableProperty.Create(nameof(ComboBoxYearsHintAssist), typeof(string), typeof(DashBoardContentPage),
            default(string));

    public string ComboBoxYearsHintAssist
    {
        get => (string)GetValue(ComboBoxYearsHintAssistProperty);
        set => SetValue(ComboBoxYearsHintAssistProperty, value);
    }

    public static readonly BindableProperty CurrentVTotalByAccountProperty =
        BindableProperty.Create(nameof(CurrentVTotalByAccount), typeof(VTotalByAccount), typeof(DashBoardContentPage),
            default(VTotalByAccount));

    public VTotalByAccount? CurrentVTotalByAccount
    {
        get => (VTotalByAccount)GetValue(CurrentVTotalByAccountProperty);
        set => SetValue(CurrentVTotalByAccountProperty, value);
    }

    public static readonly BindableProperty SelectedYearProperty = BindableProperty.Create(nameof(SelectedYear),
        typeof(string), typeof(DashBoardContentPage), default(string));

    public string SelectedYear
    {
        get => (string)GetValue(SelectedYearProperty);
        set => SetValue(SelectedYearProperty, value);
    }

    public static readonly BindableProperty SelectedMonthProperty = BindableProperty.Create(nameof(SelectedMonth),
        typeof(string), typeof(DashBoardContentPage), default(string));

    public string SelectedMonth
    {
        get => (string)GetValue(SelectedMonthProperty);
        set => SetValue(SelectedMonthProperty, value);
    }

    public ObservableCollection<string> Years { get; }
    public ObservableCollection<string> Months { get; } = [];

    public ObservableCollection<VHistory> VHistories { get; } = [];
    public ObservableCollection<VTotalByAccount> VTotalByAccounts { get; } = [];

    private static VTotalByAccount? _staticVTotalByAccount;

    private static VTotalByAccount? StaticVTotalByAccount
    {
        get => _staticVTotalByAccount;
        set
        {
            _staticVTotalByAccount = value;
            Instance.CurrentVTotalByAccount = value;
        }
    }

    private static DashBoardContentPage Instance { get; set; } = null!;

    private List<VCategoryDerive> VCategoryDerivesFilter { get; } = [];

    public ICommand CollectionViewVHistoryShortPressCommand { get; }
    private bool _isCollectionViewVHistoryLongPressInvoked;

    public ICommand CollectionViewVHistoryLongPressCommand { get; }

    public DashBoardContentPage()
    {
        CollectionViewVHistoryShortPressCommand = new Command(CollectionViewVHistory_OnShortPress);
        CollectionViewVHistoryLongPressCommand = new Command(CollectionViewVHistory_OnLongPress);

        Instance = this;

        UpdateMonthLanguage();

        var now = DateTime.Now;
        using var context = new DataBaseContext();
        Years =
        [
            ..context
                .THistories
                .Where(s => s.Date.HasValue)
                .Select(s => s.Date!.Value.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .Select(y => y.ToString())
        ];

        if (Years.Count.Equals(0))
        {
            Years.Add(DateTime.Now.Year.ToString());
        }

        SelectedYear = now.Year.ToString();
        SelectedMonth = Months[now.Month - 1];

        CurrentVTotalByAccount = context.VTotalByAccounts.FirstOrDefault();

        RefreshAccountTotal();

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private async void ButtonAddMonth_OnClick(object? sender, EventArgs e)
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(1);

        var result = UpdateFilterDate(date);

        if (result) return;

        await DisplayAlert(DashBoardContentPageResources.MessageBoxAddMonthErrorTitle,
            DashBoardContentPageResources.MessageBoxAddMonthErrorMessage,
            DashBoardContentPageResources.MessageBoxAddMonthErrorOkButton);
    }

    private void ButtonDateNow_OnClick(object? sender, EventArgs e)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);
        UpdateFilterDate(now);
    }

    private async void ButtonRemoveMonth_OnClick(object? sender, EventArgs e)
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(-1);

        var result = UpdateFilterDate(date);

        if (result) return;

        await DisplayAlert(DashBoardContentPageResources.MessageBoxRemoveMonthErrorTitle,
            DashBoardContentPageResources.MessageBoxRemoveMonthErrorMessage,
            DashBoardContentPageResources.MessageBoxRemoveMonthErrorOkButton);
    }

    private async void CollectionViewVHistory_OnLongPress(object obj)
    {
        if (obj is not VHistory vHistory) return;

        _isCollectionViewVHistoryLongPressInvoked = true;

        var history = vHistory.Id.ToISql<THistory>()!;

        var word = history.Pointed
            ? DashBoardContentPageResources.MessageBoxCollectionViewVHistoryOnLongPressUnCheck
            : DashBoardContentPageResources.MessageBoxCollectionViewVHistoryOnLongPressCheck;

        var message = string.Format(DashBoardContentPageResources.MessageBoxCollectionViewVHistoryOnLongPressMessage,
            word, $"\n{history.Description}");
        var response = await DisplayAlert(
            DashBoardContentPageResources.MessageBoxCollectionViewVHistoryOnLongPressTitle, message,
            DashBoardContentPageResources.MessageBoxCollectionViewVHistoryOnLongPressYesButton,
            DashBoardContentPageResources.MessageBoxCollectionViewVHistoryOnLongPressNoButton);
        if (response)
        {
            history.Pointed = !history.Pointed;

            if (history.Pointed) history.DatePointed = DateTime.Now;
            else history.DatePointed = null;

            Log.Information("Attention to pointed record, id: \"{HistoryId}\"", history.Id);
            history.AddOrEdit();
            Log.Information("The recording was successfully pointed");

            RefreshDataGrid();
        }

        await Task.Delay(TimeSpan.FromSeconds(1));
        _isCollectionViewVHistoryLongPressInvoked = false;
    }

    private async void CollectionViewVHistory_OnShortPress(object obj)
    {
        if (_isCollectionViewVHistoryLongPressInvoked) return;

        if (obj is not VHistory vHistory) return;

        var detailedRecordContentPage = new DetailedRecordContentPage(vHistory.Id);
        await Navigation.PushAsync(detailedRecordContentPage);

        var result = await detailedRecordContentPage.ResultDialog;
        if (result is not true) return;

        RefreshDataGrid();
        RefreshAccountTotal();
    }

    private async void CollectionViewVTotalAccount_OnLoaded(object? sender, EventArgs e)
    {
        await Dispatcher.DispatchAsync(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            RefreshRadioButtonSelected();
        });
    }

    private void CustomPicker_OnSelectedIndexChanged(object? sender, EventArgs e)
        => RefreshDataGrid();

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
    {
        UpdateLanguage();
        UpdateMonthLanguage();
    }

    private void RadioButton_OnCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        var button = sender as RadioButton;
        if (button?.BindingContext is not VTotalByAccount vTotalByAccount) return;

        StaticVTotalByAccount = vTotalByAccount;

        var name = vTotalByAccount.Name;
        if (string.IsNullOrEmpty(name)) return;

        RefreshDataGrid(name);
    }

    #endregion

    #region Function

    private DateOnly GetDateOnlyFilter()
    {
        var monthIndex = string.IsNullOrEmpty(SelectedMonth)
            ? DateTime.Now.Month
            : Months.IndexOf(SelectedMonth) + 1;

        var year = string.IsNullOrEmpty(SelectedYear)
            ? DateTime.Now.Year
            : int.Parse(SelectedYear);

        var date = DateOnly.Parse($"{year}/{monthIndex}/01");
        return date;
    }

    internal void RefreshAccountTotal()
    {
        using var context = new DataBaseContext();
        var newVTotalByAccounts = context.VTotalByAccounts.ToList();

        var itemsToDelete = VTotalByAccounts
            .Where(s => newVTotalByAccounts.All(n => n.Id != s.Id)).ToImmutableArray();

        foreach (var item in itemsToDelete)
        {
            VTotalByAccounts.Remove(item);
        }

        foreach (var vTotalByAccount in newVTotalByAccounts)
        {
            var exist = VTotalByAccounts.FirstOrDefault(s => s.Id == vTotalByAccount.Id);
            if (exist is not null)
            {
                vTotalByAccount.CopyPropertiesTo(exist);
            }
            else
            {
                VTotalByAccounts.AddAndSort(vTotalByAccount, s => s.Name!);
            }
        }
    }

    private void RefreshAccountTotal(int id)
    {
        using var context = new DataBaseContext();
        var newVTotalByAccount = context.VTotalByAccounts.FirstOrDefault(s => s.Id.Equals(id));
        if (newVTotalByAccount is null) return;

        var vTotalByAccount = VTotalByAccounts.FirstOrDefault(s => s.Id.Equals(id));
        if (vTotalByAccount is null) return;

        newVTotalByAccount.CopyPropertiesTo(vTotalByAccount);
    }

    private void RefreshDataGrid(string? accountName = null)
    {
        if (string.IsNullOrEmpty(accountName))
        {
            var radioButtons = CollectionViewVTotalAccount?.FindVisualChildren<RadioButton>().ToList() ?? [];
            if (radioButtons.Count.Equals(0)) return;

            accountName = radioButtons.FirstOrDefault(s => s.IsChecked)?.Content as string;
        }

        if (string.IsNullOrEmpty(accountName)) return;

        using var context = new DataBaseContext();

        var query = context.VHistories
            .Where(s => s.Account == accountName);

        if (!string.IsNullOrEmpty(SelectedMonth))
        {
            var monthInt = Months.IndexOf(SelectedMonth) + 1;
            query = query.Where(s => s.Date!.Value.Month.Equals(monthInt));
        }

        if (!string.IsNullOrEmpty(SelectedYear))
        {
            _ = SelectedYear.ToInt(out var yearInt);
            query = query.Where(s => s.Date!.Value.Year.Equals(yearInt));
        }

        if (VCategoryDerivesFilter.Count > 0)
        {
            var categoryName = VCategoryDerivesFilter.Select(s => s.CategoryName!);
            query = query.Where(s => categoryName.Contains(s.Category));
        }

        var records = query
            .OrderBy(s => s.Pointed)
            .ThenByDescending(s => s.Date)
            .ThenBy(s => s.Category);

        VHistories.Clear();
        VHistories.AddRange(records);
    }

    private void RefreshRadioButtonSelected()
    {
        var radioButtons = CollectionViewVTotalAccount.FindVisualChildren<RadioButton>().ToList();

        var radioButton = StaticVTotalByAccount is null
            ? radioButtons.FirstOrDefault()
            : radioButtons.FirstOrDefault(rb =>
                rb.BindingContext is VTotalByAccount vTotalByAccount &&
                vTotalByAccount.Id.Equals(StaticVTotalByAccount.Id));

        StaticVTotalByAccount = radioButton?.BindingContext as VTotalByAccount;

        if (radioButton is null) return;
        radioButton.IsChecked = true;

        RefreshDataGrid();
        RefreshAccountTotal(StaticVTotalByAccount!.Id);
    }

    private bool UpdateFilterDate(DateOnly date)
    {
        var yearStr = date.Year.ToString();
        if (!Years.Contains(yearStr)) return false;

        if (!yearStr.Equals(SelectedYear)) SelectedYear = yearStr;

        var monthIndex = date.Month - 1;
        SelectedMonth = Months[monthIndex];

        return true;
    }

    private void UpdateLanguage()
    {
        ComboBoxYearsHintAssist = DashBoardContentPageResources.ComboBoxYearsHintAssist;
        ComboBoxMonthHintAssist = DashBoardContentPageResources.ComboBoxMonthHintAssist;

        LabelTextCategory = DashBoardContentPageResources.LabelTextCategory;
        LabelTextDescription = DashBoardContentPageResources.LabelTextDescription;
        LabelTextPaymentMode = DashBoardContentPageResources.LabelTextPaymentMode;
        LabelTextValue = DashBoardContentPageResources.LabelTextValue;
    }

    private void UpdateMonthLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;

        var months = currentCulture.DateTimeFormat.MonthNames
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(s => s.ToFirstCharUpper()).ToImmutableArray();

        if (Months.Count is 0)
        {
            Months.AddRange(months);
        }
        else
        {
            var selectedMonth = Months.FirstOrDefault(month => month.Equals(SelectedMonth)) ?? string.Empty;
            for (var i = 0; i < months.Length; i++)
            {
                Months[i] = months[i];
            }

            SelectedMonth = selectedMonth;
        }
    }

    #endregion

    private async void CategoryTapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterCategory(svgPath);
    }

    private async void DescriptionTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterDescription(svgPath);
    }

    private async void PaymentModeTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterPaymentMode(svgPath);
    }

    private async void ValueTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterValue(svgPath);
    }

    private async void CategorySvgPath_OnClicked(object? sender, EventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterCategory(svgPath);
    }

    private async void DescriptionSvgPath_OnClicked(object? sender, EventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterDescription(svgPath);
    }

    private async void PaymentModeSvgPath_OnClicked(object? sender, EventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterPaymentMode(svgPath);
    }

    private async void ValueSvgPath_OnClicked(object? sender, EventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterValue(svgPath);
    }

    private List<Func<SvgPath, Task>> Filters { get; } = [];
    private List<List<VHistory>> OriginalVHistories { get; } = [];

    private async Task FilterCategory(SvgPath svgPath)
    {
        IEnumerable<int> historyIds;
        if (Filters.Count is 0) historyIds = VHistories.Select(s => s.Id);
        else
        {
            var items = Filters.Last() == FilterCategory
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            historyIds = items.Select(s => s.Id);
        }

        var mapper = Mapping.Mapper;
        await using var context = new DataBaseContext();
        var categoryTypeFk = context.THistories
            .Where(s => historyIds.Contains(s.Id))
            .Select(s => s.CategoryTypeFk!)
            .Distinct()
            .ToList();

        var vCategoryDerives = context.VCategories
            .Where(s => categoryTypeFk.Contains(s.Id))
            .Select(s => mapper.Map<VCategoryDerive>(s))
            .ToList();

        var customPopupFilterCategories = new CustomPopupFilterCategories(vCategoryDerives, VCategoryDerivesFilter);
        await this.ShowPopupAsync(customPopupFilterCategories);

        FilterManagement(svgPath, customPopupFilterCategories, FilterCategory);
    }

    // TODO work
    private async Task FilterDescription(SvgPath svgPath)
    {
        var popup = new CustomPopup();
        await this.ShowPopupAsync(popup);

        svgPath.GeometrySource = EPackIcons.FilterCheck;
    }

    // TODO work
    private async Task FilterPaymentMode(SvgPath svgPath)
    {
        var popup = new CustomPopup();
        await this.ShowPopupAsync(popup);

        svgPath.GeometrySource = EPackIcons.FilterCheck;
    }

    // TODO work
    private async Task FilterValue(SvgPath svgPath)
    {
        var popup = new CustomPopup();
        await this.ShowPopupAsync(popup);

        svgPath.GeometrySource = EPackIcons.FilterCheck;
    }

    private static SvgPath? FindSvgPath(object? sender)
    {
        return sender switch
        {
            null => null,
            SvgPath svgPath => svgPath,
            _ => sender is HorizontalStackLayout horizontalStackLayout
                ? horizontalStackLayout.FindVisualChildren<SvgPath>().FirstOrDefault()
                : null
        };
    }

    private bool RefreshFilter<T>(List<T> collection, ICustomPopupFilter<T> customPopupFilter, SvgPath svgPath)
    {
        collection.Clear();
        collection.AddRange(customPopupFilter.GetFilteredItemChecked());

        var itemCheckedCount = customPopupFilter.GetFilteredItemCheckedCount();
        var itemCount = customPopupFilter.GetFilteredItemCount();

        var icon = itemCheckedCount is 0 || itemCheckedCount.Equals(itemCount)
            ? EPackIcons.Filter
            : EPackIcons.FilterCheck;

        svgPath.GeometrySource = icon;

        RefreshDataGrid();

        return icon is EPackIcons.FilterCheck;
    }

    private void FilterManagement(SvgPath svgPath, CustomPopupFilterCategories customPopupFilterCategories,
        Func<SvgPath, Task> filterFunc)
    {
        if (Filters.Count is 0 || Filters.Last() != filterFunc)
        {
            Filters.Add(filterFunc);
            OriginalVHistories.Add(VHistories.ToList());
        }

        var isActive = RefreshFilter(VCategoryDerivesFilter, customPopupFilterCategories, svgPath);

        if (!isActive && Filters.Last() == filterFunc)
        {
            var lastIndex = Filters.Count - 1;
            Filters.RemoveAt(lastIndex);

            lastIndex = OriginalVHistories.Count - 1;
            OriginalVHistories.RemoveAt(lastIndex);
        }
    }
}