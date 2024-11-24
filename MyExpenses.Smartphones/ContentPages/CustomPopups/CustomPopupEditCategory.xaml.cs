using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupEditCategory
{
    public static readonly BindableProperty ButtonCancelTextProperty = BindableProperty.Create(nameof(ButtonCancelText),
        typeof(string), typeof(CustomPopupEditCategory), default(string));

    public string ButtonCancelText
    {
        get => (string)GetValue(ButtonCancelTextProperty);
        set => SetValue(ButtonCancelTextProperty, value);
    }

    public static readonly BindableProperty ButtonDeleteTextProperty = BindableProperty.Create(nameof(ButtonDeleteText),
        typeof(string), typeof(CustomPopupEditCategory), default(string));

    public string ButtonDeleteText
    {
        get => (string)GetValue(ButtonDeleteTextProperty);
        set => SetValue(ButtonDeleteTextProperty, value);
    }

    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(CustomPopupEditCategory), default(string));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty CanDeleteProperty = BindableProperty.Create(nameof(CanDelete), typeof(bool),
        typeof(CustomPopupEditCategory), default(bool));

    public bool CanDelete
    {
        get => (bool)GetValue(CanDeleteProperty);
        set => SetValue(CanDeleteProperty, value);
    }

    public static readonly BindableProperty LabelTextColorProperty = BindableProperty.Create(nameof(LabelTextColor),
        typeof(string), typeof(CustomPopupEditCategory), default(string));

    public string LabelTextColor
    {
        get => (string)GetValue(LabelTextColorProperty);
        set => SetValue(LabelTextColorProperty, value);
    }

    public static readonly BindableProperty MaxLenghtProperty =
        BindableProperty.Create(nameof(MaxLenght), typeof(int), typeof(CustomPopupEditCategory), default(int));

    public int MaxLenght
    {
        get => (int)GetValue(MaxLenghtProperty);
        set => SetValue(MaxLenghtProperty, value);
    }

    public static readonly BindableProperty EntryTextProperty = BindableProperty.Create(nameof(EntryText),
        typeof(string), typeof(CustomPopupEditCategory), default(string));

    public string EntryText
    {
        get => (string)GetValue(EntryTextProperty);
        set => SetValue(EntryTextProperty, value);
    }


    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(CustomPopupEditCategory), default(string));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty SelectedHexadecimalColorCodeProperty =
        BindableProperty.Create(nameof(SelectedHexadecimalColorCode), typeof(string), typeof(CustomPopupEditCategory),
            "#00000000");

    public string SelectedHexadecimalColorCode
    {
        get => (string)GetValue(SelectedHexadecimalColorCodeProperty);
        set => SetValue(SelectedHexadecimalColorCodeProperty, value);
    }

    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(nameof(SelectedColor),
        typeof(TColor), typeof(CustomPopupEditCategory), default(TColor));

    public TColor? SelectedColor
    {
        get => (TColor?)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public List<TColor> Colors { get; }

    private readonly TaskCompletionSource<ECustomPopupEntryResult> _taskCompletionSource = new();

    public Task<ECustomPopupEntryResult> ResultDialog
        => _taskCompletionSource.Task;

    public CustomPopupEditCategory()
    {
        using var context = new DataBaseContext();
        Colors = [..context.TColors.OrderBy(s => s.Name)];

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        PlaceholderText = "PlaceholderText";
        LabelTextColor = "LabelTextColor";

        ButtonValidText = "ButtonValidText";
        ButtonDeleteText = "ButtonDeleteText";
        ButtonCancelText = "ButtonCancelText";
    }

    private void PickerColor_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        SelectedHexadecimalColorCode = SelectedColor is null
            ? "#00000000"
            : SelectedColor.HexadecimalColorCode!;
    }


    public void SetVCategory(VCategory category)
    {
        var color = Colors.First(s => s.Id.Equals(category.ColorFk));
        SelectedColor = color;
        SelectedHexadecimalColorCode = color.HexadecimalColorCode!;
        EntryText = category.CategoryName!;
    }

    private void ButtonValid_OnClicked(object? sender, EventArgs e)
        => SetDialogueResult(ECustomPopupEntryResult.Valid);

    private void ButtonDelete_OnClicked(object? sender, EventArgs e)
        => SetDialogueResult(ECustomPopupEntryResult.Delete);

    private void ButtonCancel_OnClicked(object? sender, EventArgs e)
        => SetDialogueResult(ECustomPopupEntryResult.Cancel);

    private void SetDialogueResult(ECustomPopupEntryResult customPopupEntryResult)
    {
        _taskCompletionSource.SetResult(customPopupEntryResult);
        Close();
    }
}