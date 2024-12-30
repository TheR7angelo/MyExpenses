using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CustomPopups.CustomPopupEntry;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupEntry
{
    public static readonly BindableProperty ButtonCancelTextProperty = BindableProperty.Create(nameof(ButtonCancelText),
        typeof(string), typeof(CustomPopupEntry));

    public string ButtonCancelText
    {
        get => (string)GetValue(ButtonCancelTextProperty);
        set => SetValue(ButtonCancelTextProperty, value);
    }

    public static readonly BindableProperty ButtonDeleteTextProperty = BindableProperty.Create(nameof(ButtonDeleteText),
        typeof(string), typeof(CustomPopupEntry));

    public string ButtonDeleteText
    {
        get => (string)GetValue(ButtonDeleteTextProperty);
        set => SetValue(ButtonDeleteTextProperty, value);
    }

    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(CustomPopupEntry));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty MaxLenghtProperty =
        BindableProperty.Create(nameof(MaxLenght), typeof(int), typeof(CustomPopupEntry), 255);

    public int MaxLenght
    {
        get => (int)GetValue(MaxLenghtProperty);
        init => SetValue(MaxLenghtProperty, value);
    }

    public static readonly BindableProperty HasMultilineProperty =
        BindableProperty.Create(nameof(HasMultiline), typeof(string), typeof(CustomPopupEntry));

    public string HasMultiline
    {
        get => (string)GetValue(HasMultilineProperty);
        init => SetValue(HasMultilineProperty, value);
    }

    public static readonly BindableProperty HasClearButtonProperty =
        BindableProperty.Create(nameof(HasClearButton), typeof(bool), typeof(CustomPopupEntry), true);

    public bool HasClearButton
    {
        get => (bool)GetValue(HasClearButtonProperty);
        init => SetValue(HasClearButtonProperty, value);
    }

    private static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(EntryText), typeof(string), typeof(CustomPopupEntry));

    public string EntryText
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(CustomPopupEntry));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        init => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty CanDeleteProperty =
        BindableProperty.Create(nameof(CanDelete), typeof(bool), typeof(CustomPopupEntry), false);

    public bool CanDelete
    {
        get => (bool)GetValue(CanDeleteProperty);
        init => SetValue(CanDeleteProperty, value);
    }

    private readonly TaskCompletionSource<ECustomPopupEntryResult> _taskCompletionSource = new();

    public Task<ECustomPopupEntryResult> ResultDialog
        => _taskCompletionSource.Task;

    public CustomPopupEntry()
    {
        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonValid_OnClicked(object? sender, EventArgs e)
        => SetDialogueResult(ECustomPopupEntryResult.Valid);

    private void ButtonDelete_OnClicked(object? sender, EventArgs e)
        => SetDialogueResult(ECustomPopupEntryResult.Delete);

    private void ButtonCancel_OnClicked(object? sender, EventArgs e)
        => SetDialogueResult(ECustomPopupEntryResult.Cancel);

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    private void UpdateLanguage()
    {
        ButtonValidText = CustomPopupEntryResources.ButtonValidText;
        ButtonDeleteText = CustomPopupEntryResources.ButtonDeleteText;
        ButtonCancelText = CustomPopupEntryResources.ButtonCancelText;
    }

    private void SetDialogueResult(ECustomPopupEntryResult customPopupEntryResult)
    {
        _taskCompletionSource.SetResult(customPopupEntryResult);
        Close();
    }

    #endregion
}