using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.SharedUtils.Resources.Resx.PopupEntryManagement;

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
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(MaxLenght), typeof(int), typeof(CustomPopupEntry), 255);

    public int MaxLenght
    {
        get => (int)GetValue(MaxLenghtProperty);
        // ReSharper disable once HeapView.BoxingAllocation
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
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(HasClearButton), typeof(bool), typeof(CustomPopupEntry), true);

    public bool HasClearButton
    {
        get => (bool)GetValue(HasClearButtonProperty);
        // ReSharper disable once HeapView.BoxingAllocation
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
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(CanDelete), typeof(bool), typeof(CustomPopupEntry), false);

    public bool CanDelete
    {
        get => (bool)GetValue(CanDeleteProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        init => SetValue(CanDeleteProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // Necessary allocation of TaskCompletionSource to manage the asynchronous result of the popup dialog.
    // This allows the dialog to communicate its selected result (Cancel, Delete, Valid) back to the caller
    // and acts as a bridge between UI actions and the task-based asynchronous code.
    private readonly TaskCompletionSource<ECustomPopupEntryResult> _taskCompletionSource = new();

    public Task<ECustomPopupEntryResult> ResultDialog
        => _taskCompletionSource.Task;

    public CustomPopupEntry()
    {
        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
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
        ButtonValidText = PopupEntryManagementResources.ButtonValidText;
        ButtonDeleteText = PopupEntryManagementResources.ButtonDeleteText;
        ButtonCancelText = PopupEntryManagementResources.ButtonCancelText;
    }

    private void SetDialogueResult(ECustomPopupEntryResult customPopupEntryResult)
    {
        _taskCompletionSource.SetResult(customPopupEntryResult);
        // TODO work
        // Close();
    }

    #endregion
}