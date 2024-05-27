using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Wpf.Resources.Regex;
using MyExpenses.Wpf.Utils.Maps;
using MyExpenses.Wpf.Windows;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class RecordExpensePage
{

    public THistory History { get; } = new();

    public string SelectedValuePathAccount { get; } = nameof(TAccount.Id);
    public string DisplayMemberPathAccount { get; } = nameof(TAccount.Name);
    public string SelectedValuePathCategoryType { get; } = nameof(TCategoryType.Id);
    public string DisplayMemberPathCategoryType { get; } = nameof(TCategoryType.Name);
    public string SelectedValuePathModePayment { get; } = nameof(TModePayment.Id);
    public string DisplayMemberPathModePayment { get; } = nameof(TModePayment.Name);
    public string SelectedValuePathPlace { get; } = nameof(TPlace.Id);
    public string DisplayMemberPathPlace { get; } = nameof(TPlace.Name);

    //TODO work
    public string ComboBoxAccountHintAssist { get; } = "From account :";
    //TODO work
    public string TextBoxDescriptionHintAssist { get; } = "Description :";
    //TODO work
    public string ComboBoxCategoryTypeHintAssist { get; } = "Category type :";
    //TODO work
    public string ComboBoxModePaymentHintAssist { get; } = "Mode payment :";
    //TODO work
    public string TextBoxValueHintAssist { get; } = "Value :";
    //TODO work
    public string DatePickerWhenHintAssist { get; } = "Date :";
    //TODO work
    public string TimePickerWhenHintAssist { get; } = "Time :";
    //TODO work
    public string ComboBoxPlaceHintAssist { get; } = "Place :";
    //TODO work
    public string CheckBoxPointedContent { get; } = "Is pointed :";
    //TODO work
    public string ComboBoxBackgroundHintAssist { get; } = "Basemap :";

    public required DashBoardPage DashBoardPage { get; set; }

    public ObservableCollection<TAccount> Accounts { get; }
    public ObservableCollection<TCategoryType> CategoryTypes { get; }
    public ObservableCollection<TModePayment> ModePayments { get; }
    public ObservableCollection<TPlace> Places { get; }

    private WritableLayer PlaceLayer { get; } = new() { Style = null, IsMapInfoLayer = true, Tag = typeof(TPlace) };
    public List<KnownTileSource> KnownTileSources { get; }
    public KnownTileSource KnownTileSourceSelected { get; set; }

    public RecordExpensePage()
    {
        KnownTileSources = [..MapsuiMapExtensions.GetAllKnowTileSource()];

        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];
        CategoryTypes = [..context.TCategoryTypes.OrderBy(s => s.Name)];
        ModePayments = [..context.TModePayments.OrderBy(s => s.Name)];
        Places = [..context.TPlaces.OrderBy(s => s.Name)];

        // TODO add listener color change
        var brush = (SolidColorBrush)FindResource("MaterialDesignPaper");
        var backColor = brush.ToMapsuiColor();

        var map = MapsuiMapExtensions.GetMap(true, backColor);
        map.Layers.Add(PlaceLayer);

        InitializeComponent();

        var language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        DatePicker.Language = language;

        var configuration = MyExpenses.Utils.Config.Configuration;
        TimePicker.Is24Hours = configuration.Interface.Clock.Is24Hours;

        MapControl.Map = map;
    }

    #region Action

    private void ButtonAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditAccountWindow = new AddEditAccountWindow();

        var account = History.AccountFk?.ToTAccount();
        if (account is not null) addEditAccountWindow.SetTAccount(account);

        addEditAccountWindow.ShowDialog();
        if (addEditAccountWindow.DialogResult != true) return;

        if (addEditAccountWindow.DeleteAccount)
        {
            var accountToRemove = Accounts.FirstOrDefault(s => s.Id == History.AccountFk);
            if (accountToRemove is not null) Accounts.Remove(accountToRemove);

            DashBoardPage.RefreshAccountTotal();
            DashBoardPage.RefreshRadioButtonSelected();
        }
        else
        {
            var editedAccount = addEditAccountWindow.Account;

            Log.Information("Attempting to edit the account \"{AccountName}\"", editedAccount.Name);
            var (success, exception) = editedAccount.AddOrEdit();
            if (success)
            {
                Log.Information("Account was successfully edited");
                var json = editedAccount.ToJsonString();
                Log.Information("{Json}", json);

                //TODO work
                MsgBox.Show("Account was successfully edited", MsgBoxImage.Check);

                var accountToRemove = Accounts.FirstOrDefault(s => s.Id == History.Id);
                Accounts!.AddAndSort(accountToRemove, editedAccount, s => s?.Name!);

                History.AccountFk = editedAccount.Id;
            }
            else
            {
                //TODO work
                Log.Error(exception, "An error occurred please retry");
                MsgBox.Show("An error occurred please retry", MsgBoxImage.Warning);
            }
        }
    }

    //TODO work
    private void ButtonCategoryType_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditCategoryTypeWindow = new AddEditCategoryTypeWindow();
        var categoryType = History.CategoryTypeFk?.ToCategoryType();
        if (categoryType is not null) addEditCategoryTypeWindow.SetTCategoryType(categoryType);

        var result = addEditCategoryTypeWindow.ShowDialog();
        if (result != true) return;

        if (addEditCategoryTypeWindow.CategoryTypeDeleted)
        {
            var categoryTypeToRemove = CategoryTypes.FirstOrDefault(s => s.Id == History.CategoryTypeFk);
            if (categoryTypeToRemove is not null) CategoryTypes.Remove(categoryTypeToRemove);

            DashBoardPage.RefreshAccountTotal();
            DashBoardPage.RefreshRadioButtonSelected();
        }
        else
        {
            var editedCategoryType = addEditCategoryTypeWindow.CategoryType;
            Log.Information("Attempting to edit the category type id: {Id}", editedCategoryType.Id);

            var editedCategoryTypeDeepCopy = editedCategoryType.DeepCopy();

            var (success, exception) = editedCategoryType.AddOrEdit();
            if (success)
            {
                using var context = new DataBaseContext();
                editedCategoryTypeDeepCopy.ColorFkNavigation =
                    context.TColors.FirstOrDefault(s => s.Id == editedCategoryTypeDeepCopy.ColorFk);

                CategoryTypes!.AddAndSort(categoryType, editedCategoryTypeDeepCopy, s => s!.Name!);

                Log.Information("Category type was successfully edited");
                var json = editedCategoryTypeDeepCopy.ToJsonString();
                Log.Information("{Json}", json);

                //TODO work
                MsgBox.Show("Category type was successfully edited", MsgBoxImage.Check);

                DashBoardPage.RefreshAccountTotal();
                DashBoardPage.RefreshRadioButtonSelected();
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MsgBox.Show("An error occurred please retry", MsgBoxImage.Error);
            }
        }
    }

    //TODO work
    private void ButtonModePayment_OnClick(object sender, RoutedEventArgs e)
    {

    }

    //TODO work
    private void ButtonPlace_OnClick(object sender, RoutedEventArgs e)
    {

    }

    private void TextBoxValue_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text;

        if (double.TryParse(txt, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            History.Value = value;
        else if (!txt.EndsWith('.')) History.Value = null;
    }

    private void MapControl_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateTileLayer();
    
    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateTileLayer();

    private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text.Insert(textBox.SelectionStart, e.Text);

        e.Handled = txt.IsOnlyDecimal();
    }

    #endregion

    #region Function

    private void UpdateTileLayer()
    {
        const string layerName = "Background";

        var httpTileSource = BruTile.Predefined.KnownTileSources.Create(KnownTileSourceSelected);
        var tileLayer = new TileLayer(httpTileSource);
        tileLayer.Name = layerName;

        var layers = MapControl?.Map.Layers.FindLayer(layerName);
        if (layers is not null) MapControl?.Map.Layers.Remove(layers.ToArray());

        MapControl?.Map.Layers.Insert(0, tileLayer);
    }

    #endregion
}