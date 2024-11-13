using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using MyExpenses.Models.Sql.Derivatives.Tables;
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
    public static readonly BindableProperty RecordFoundOnProperty = BindableProperty.Create(nameof(RecordFoundOn),
        typeof(string), typeof(DashBoardContentPage), default(string));

    public string RecordFoundOn
    {
        get => (string)GetValue(RecordFoundOnProperty);
        set => SetValue(RecordFoundOnProperty, value);
    }

    public static readonly BindableProperty RowTotalCountProperty =
        BindableProperty.Create(nameof(RowTotalCount), typeof(int), typeof(DashBoardContentPage), default(int));

    public int RowTotalCount
    {
        get => (int)GetValue(RowTotalCountProperty);
        set => SetValue(RowTotalCountProperty, value);
    }

    public static readonly BindableProperty RowTotalFilteredCountProperty =
        BindableProperty.Create(nameof(RowTotalFilteredCount), typeof(int), typeof(DashBoardContentPage), default(int));

    public int RowTotalFilteredCount
    {
        get => (int)GetValue(RowTotalFilteredCountProperty);
        set => SetValue(RowTotalFilteredCountProperty, value);
    }

    public static readonly BindableProperty ElapsedTimeProperty = BindableProperty.Create(
        nameof(ElapsedTimeLoadingData),
        typeof(string), typeof(DashBoardContentPage), default(string));

    public string ElapsedTimeLoadingData
    {
        get => (string)GetValue(ElapsedTimeProperty);
        set => SetValue(ElapsedTimeProperty, value);
    }

    public static readonly BindableProperty ElapsedTimeLoadingDataTextProperty =
        BindableProperty.Create(nameof(ElapsedTimeLoadingDataText), typeof(string), typeof(DashBoardContentPage),
            default(string));

    public string ElapsedTimeLoadingDataText
    {
        get => (string)GetValue(ElapsedTimeLoadingDataTextProperty);
        set => SetValue(ElapsedTimeLoadingDataTextProperty, value);
    }

    public static readonly BindableProperty LabelTextPlaceProperty = BindableProperty.Create(nameof(LabelTextPlace),
        typeof(string), typeof(DashBoardContentPage), default(string));

    public string LabelTextPlace
    {
        get => (string)GetValue(LabelTextPlaceProperty);
        set => SetValue(LabelTextPlaceProperty, value);
    }

    public static readonly BindableProperty LabelTextCheckedProperty = BindableProperty.Create(nameof(LabelTextChecked),
        typeof(string), typeof(DashBoardContentPage), default(string));

    public string LabelTextChecked
    {
        get => (string)GetValue(LabelTextCheckedProperty);
        set => SetValue(LabelTextCheckedProperty, value);
    }

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
    private List<StringIsChecked> HistoryDescriptions { get; } = [];
    private List<TModePaymentDerive> ModePaymentDeriveFilter { get; } = [];
    private List<DoubleIsChecked> HistoryValues { get; } = [];
    private List<BoolIsChecked> HistoryChecked { get; } = [];
    private List<TPlaceDerive> PlaceDeriveFilter { get; } = [];

    private List<EFilter> Filters { get; } = [];
    private List<List<VHistory>> OriginalVHistories { get; } = [];

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

    private async void CategoryTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => await RunFilter(sender, FilterCategory);

    private async void CategorySvgPath_OnClicked(object? sender, EventArgs e)
        => await RunFilter(sender, FilterCategory);

    private async void CollectionViewVHistory_OnLongPress(object obj)
    {
        if (obj is not VHistory vHistory) return;

        _isCollectionViewVHistoryLongPressInvoked = true;

        var history = vHistory.Id.ToISql<THistory>()!;

        var word = history.IsPointed
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
            history.IsPointed = !history.IsPointed;

            if (history.IsPointed) history.DatePointed = DateTime.Now;
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

        var detailedRecordContentPage = new DetailedRecordContentPage(vHistory.Id) { CanBeDeleted = true };
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

    private async void DescriptionTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => await RunFilter(sender, FilterDescription);

    private async void DescriptionSvgPath_OnClicked(object? sender, EventArgs e)
        => await RunFilter(sender, FilterDescription);

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
    {
        UpdateLanguage();
        UpdateMonthLanguage();
    }

    private async void PaymentModeTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => await RunFilter(sender, FilterPaymentMode);

    private async void PaymentModeSvgPath_OnClicked(object? sender, EventArgs e)
        => await RunFilter(sender, FilterPaymentMode);

    private async void PlaceTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => await RunFilter(sender, FilterPlace);

    private async void PlaceSvgPath_OnClicked(object? sender, EventArgs e)
        => await RunFilter(sender, FilterPlace);

    private async void PointedTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => await RunFilter(sender, FilterChecked);

    private async void PointedSvgPath_OnClicked(object? sender, EventArgs e)
        => await RunFilter(sender, FilterChecked);

    private void SvgPathRefresh_OnClicked(object? sender, EventArgs e)
    {
        if (sender is not SvgPath svgPath) return;
        if (svgPath.Parent is not HorizontalStackLayout horizontalStackLayout) return;

        var horizontalStackLayoutChildren = horizontalStackLayout.FindVisualChildren<HorizontalStackLayout>();
        foreach (var horizontalStackLayoutChild in horizontalStackLayoutChildren)
        {
            var svgPathChild = horizontalStackLayoutChild.FindVisualChildren<SvgPath>().FirstOrDefault();
            if (svgPathChild is null) continue;

            svgPathChild.GeometrySource = EPackIcons.Filter;
        }

        VCategoryDerivesFilter.Clear();
        HistoryDescriptions.Clear();
        ModePaymentDeriveFilter.Clear();
        HistoryValues.Clear();
        HistoryChecked.Clear();
        PlaceDeriveFilter.Clear();

        RefreshDataGrid();
    }

    private async void ValueTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => await RunFilter(sender, FilterValue);


    private async void ValueSvgPath_OnClicked(object? sender, EventArgs e)
        => await RunFilter(sender, FilterValue);

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

    private async Task FilterCategory(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Category;

        IEnumerable<int> historyIds;
        if (Filters.Count is 0) historyIds = VHistories.Select(s => s.Id);
        else
        {
            var items = Filters.Last() == eFilter
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
            .OrderBy(s => s.CategoryName)
            .Select(s => mapper.Map<VCategoryDerive>(s))
            .ToList();

        var customPopupFilterCategories = new CustomPopupFilterCategories(vCategoryDerives, VCategoryDerivesFilter);
        await this.ShowPopupAsync(customPopupFilterCategories);

        FilterManagement(VCategoryDerivesFilter, customPopupFilterCategories, eFilter, svgPath);
    }

    private async Task FilterChecked(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Checked;

        IEnumerable<BoolIsChecked> isCheckeds;
        if (Filters.Count is 0) isCheckeds = VHistories.Select(s => new BoolIsChecked { BoolValue = (bool)s.IsPointed! });
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            isCheckeds = items.Select(s => new BoolIsChecked { BoolValue = (bool)s.IsPointed! });
        }

        isCheckeds = isCheckeds.Distinct();
        isCheckeds = isCheckeds.OrderBy(s => s.BoolValue);

        var customPopupFilterChecked = new CustomPopupFilterChecked(isCheckeds, HistoryChecked);
        await this.ShowPopupAsync(customPopupFilterChecked);

        FilterManagement(HistoryChecked, customPopupFilterChecked, eFilter, svgPath);
    }

    private async Task FilterDescription(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Description;

        IEnumerable<StringIsChecked> historyDescription;
        if (Filters.Count is 0) historyDescription = VHistories.Select(s => new StringIsChecked { StringValue = s.Description });
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            historyDescription = items.Select(s => new StringIsChecked { StringValue = s.Description });
        }

        historyDescription = historyDescription.Distinct();
        historyDescription = historyDescription.OrderBy(s => s.StringValue);

        var customPopupFilterDescription = new CustomPopupFilterDescriptions(historyDescription, HistoryDescriptions);
        await this.ShowPopupAsync(customPopupFilterDescription);

        FilterManagement(HistoryDescriptions, customPopupFilterDescription, eFilter, svgPath);
    }

    private void FilterManagement<T>(List<T> collection, ICustomPopupFilter<T> customPopupFilter, EFilter eFilter,
        SvgPath svgPath)
    {
        if (Filters.Count is 0 || Filters.Last() != eFilter)
        {
            Filters.Add(eFilter);
            OriginalVHistories.Add(VHistories.ToList());
        }

        var isActive = RefreshFilter(collection, customPopupFilter, svgPath);

        if (!isActive && Filters.Last() == eFilter)
        {
            var lastIndex = Filters.Count - 1;
            Filters.RemoveAt(lastIndex);

            lastIndex = OriginalVHistories.Count - 1;
            OriginalVHistories.RemoveAt(lastIndex);
        }
    }

    private async Task FilterPaymentMode(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.PaymentMode;

        IEnumerable<int> historyIds;
        if (Filters.Count is 0) historyIds = VHistories.Select(s => s.Id);
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            historyIds = items.Select(s => s.Id);
        }

        var mapper = Mapping.Mapper;
        await using var context = new DataBaseContext();
        var modePaymentFk = context.THistories
            .Where(s => historyIds.Contains(s.Id))
            .Select(s => s.ModePaymentFk!)
            .Distinct()
            .ToList();

        var tModePaymentDerives = context.TModePayments
            .Where(s => modePaymentFk.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => mapper.Map<TModePaymentDerive>(s))
            .ToList();

        var customPopupFilterModePayment =
            new CustomPopupFilterModePayments(tModePaymentDerives, ModePaymentDeriveFilter);
        await this.ShowPopupAsync(customPopupFilterModePayment);

        FilterManagement(ModePaymentDeriveFilter, customPopupFilterModePayment, eFilter, svgPath);
    }

    private async Task FilterPlace(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Place;

        IEnumerable<int> historyIds;
        if (Filters.Count is 0) historyIds = VHistories.Select(s => s.Id);
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            historyIds = items.Select(s => s.Id);
        }

        var mapper = Mapping.Mapper;
        await using var context = new DataBaseContext();
        var placeFk = context.THistories
            .Where(s => historyIds.Contains(s.Id))
            .Select(s => s.PlaceFk!)
            .Distinct()
            .ToList();

        var placeDerives = context.TPlaces
            .Where(s => placeFk.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => mapper.Map<TPlaceDerive>(s))
            .ToList();

        var customPopupFilterPlaces = new CustomPopupFilterPlaces(placeDerives, PlaceDeriveFilter);
        await this.ShowPopupAsync(customPopupFilterPlaces);

        FilterManagement(PlaceDeriveFilter, customPopupFilterPlaces, eFilter, svgPath);
    }

    private async Task FilterValue(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Value;

        IEnumerable<DoubleIsChecked> historyValues;
        if (Filters.Count is 0) historyValues = VHistories.Select(s => new DoubleIsChecked { DoubleValue = s.Value });
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            historyValues = items.Select(s => new DoubleIsChecked { DoubleValue = s.Value });
        }

        historyValues = historyValues.Distinct();
        historyValues = historyValues.OrderBy(s => s.DoubleValue);

        var customPopupFilterDoubleValues = new CustomPopupFilterDoubleValues(historyValues, HistoryValues);
        await this.ShowPopupAsync(customPopupFilterDoubleValues);

        FilterManagement(HistoryValues, customPopupFilterDoubleValues, eFilter, svgPath);
    }

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

        var stopwatch = new Stopwatch();
        stopwatch.Start();

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

        RowTotalCount = query.Count();

        if (VCategoryDerivesFilter.Count > 0)
        {
            var categoryName = VCategoryDerivesFilter.Select(s => s.CategoryName!);
            query = query.Where(s => categoryName.Contains(s.Category));
        }

        if (HistoryDescriptions.Count > 0)
        {
            var historyDescriptions = HistoryDescriptions.Select(s => s.StringValue);
            query = query.Where(s => historyDescriptions.Contains(s.Description));
        }

        if (ModePaymentDeriveFilter.Count > 0)
        {
            var modePayments = ModePaymentDeriveFilter.Select(s => s.Name);
            query = query.Where(s => modePayments.Contains(s.ModePayment));
        }

        if (HistoryValues.Count > 0)
        {
            var historyValues = HistoryValues.Select(s => s.DoubleValue);
            query = query.Where(s => historyValues.Contains(s.Value));
        }

        if (HistoryChecked.Count > 0)
        {
            var historyChecked = HistoryChecked.Select(s => s.BoolValue);
            query = query.Where(s => historyChecked.Contains((bool)s.IsPointed!));
        }

        if (PlaceDeriveFilter.Count > 0)
        {
            var historyPlaces = PlaceDeriveFilter.Select(s => s.Name);
            query = query.Where(s => historyPlaces.Contains(s.Place));
        }

        RowTotalFilteredCount = query.Count();

        var records = query
            .OrderBy(s => s.IsPointed)
            .ThenByDescending(s => s.Date)
            .ThenBy(s => s.Category);

        stopwatch.Stop();
        ElapsedTimeLoadingData = stopwatch.Elapsed.ToString("hh\\:mm\\:ss");

        VHistories.Clear();
        VHistories.AddRange(records);
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
        LabelTextChecked = DashBoardContentPageResources.LabelTextChecked;
        LabelTextPlace = DashBoardContentPageResources.LabelTextPlace;

        ElapsedTimeLoadingDataText = $"{DashBoardContentPageResources.ElapsedTimeLoadingDataText} ";
        RecordFoundOn = $" {DashBoardContentPageResources.RecordFoundOn} ";
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

    private static async Task RunFilter(object? sender, Func<SvgPath, Task> func)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await func.Invoke(svgPath);
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

    private enum EFilter
    {
        Category,
        Description,
        PaymentMode,
        Value,
        Checked,
        Place
    }
}