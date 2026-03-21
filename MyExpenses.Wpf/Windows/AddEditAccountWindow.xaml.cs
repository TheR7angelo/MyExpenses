using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Categories;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.RegexUtils;
using MyExpenses.SharedUtils.Resources.Resx.AccountTypeManagement;
using MyExpenses.SharedUtils.Resources.Resx.AddEditAccount;
using MyExpenses.SharedUtils.Resources.Resx.CategoryTypesManagement;
using MyExpenses.SharedUtils.Resources.Resx.CurrencySymbolManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditAccountWindow
{
    #region DependecyProperty

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EnableStartingBalanceProperty =
        DependencyProperty.Register(nameof(EnableStartingBalance), typeof(bool), typeof(AddEditAccountWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EnableStartingBalance
    {
        get => (bool)GetValue(EnableStartingBalanceProperty);
        set => SetValue(EnableStartingBalanceProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EditAccountProperty =
        DependencyProperty.Register(nameof(EditAccount), typeof(bool), typeof(AddEditAccountWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EditAccount
    {
        get => (bool)GetValue(EditAccountProperty);
        set => SetValue(EditAccountProperty, value);
    }

    #endregion

    #region Property

    public bool DeleteAccount { get; private set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TAccount Account { get; } = new();
    public AccountViewModel AccountViewModel { get; } = new();
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public THistory History { get; } = new() { IsPointed = true };

    public static string SelectedValuePathCategoryType => nameof(TCategoryType.Id);

    public ObservableCollection<AccountTypeViewModel> AccountTypes { get; } = [];
    public ObservableCollection<CurrencyViewModel> Currencies { get; } = [];
    public ObservableCollection<CategoryTypeViewModel> CategoryTypes { get; } = [];

    #endregion

    private readonly IAccountPresentationService _accountPresentationService;
    private readonly IAccountPresentationValidationService _accountPresentationValidationService;
    private readonly ICategoryPresentationService _categoryPresentationService;

    public AddEditAccountWindow(IAccountPresentationService accountPresentationService,
        IAccountPresentationValidationService accountPresentationValidationService,
        ICategoryPresentationService categoryPresentationService)
    {
        _accountPresentationService = accountPresentationService;
        _accountPresentationValidationService = accountPresentationValidationService;
        _categoryPresentationService = categoryPresentationService;

        _ = FillCollection();

        InitializeComponent();
    }

    #region Action

    private void ButtonAddAccountType_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO injector DTO MODEL VIEW
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var addEditAccountType = new AddEditAccountTypeWindow();
        var result = addEditAccountType.ShowDialog();
        if (result is not true) return;

        var newAccountType = addEditAccountType.AccountType;

        Log.Information("Attempting to inject the new account type \"{NewAccountTypeName}\"", newAccountType.Name);
        var (success, exception) = newAccountType.AddOrEdit();
        if (success)
        {
            // TODO correct
            // AccountTypes.AddAndSort(newAccountType, s => s.Name);
            // Account.AccountTypeFk = newAccountType.Id;

            Log.Information("Account type was successfully added");
            var json = newAccountType.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxAddNewAccountTypeSuccessTitle,
                AccountTypeManagementResources.MessageBoxAddNewAccountTypeSuccessMessage, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxAddNewAccountTypeErrorTitle,
                AccountTypeManagementResources.MessageBoxAddNewAccountTypeErrorMessage, MsgBoxImage.Error);
        }
    }

    private void ButtonAddCategoryType_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var addEditCategoryType = new AddEditCategoryTypeWindow();
        var result = addEditCategoryType.ShowDialog();
        if (result is not true) return;

        var newCategoryType = addEditCategoryType.CategoryType;

        Log.Information("Attempting to inject the new category type \"{NewCategoryTypeName}\"", newCategoryType.Name);
        var (success, exception) = newCategoryType.AddOrEdit();
        if (success)
        {
            // TODO correct
            // CategoryTypes.AddAndSort(newCategoryType, s => s.Name!);
            // History.CategoryTypeFk = newCategoryType.Id;

            Log.Information("Account type was successfully added");
            var json = newCategoryType.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxAddNewCategoryTypeSuccessTitle,
                CategoryTypesManagementResources.MessageBoxAddNewCategoryTypeSuccessMessage, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.MsgBox.Show(CategoryTypesManagementResources.MessageBoxAddNewCategoryTypeErrorTitle,
                CategoryTypesManagementResources.MessageBoxAddNewCategoryTypeErrorMessage, MsgBoxImage.Error);
        }
    }

    private void ButtonAddCurrency_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var addEditCurrency = new AddEditCurrencyWindow();
        var result = addEditCurrency.ShowDialog();
        if (result is not true) return;

        var newCurrency = addEditCurrency.Currency;

        Log.Information("Attempting to inject the new currency symbole \"{NewCurrencySymbole}\"", newCurrency.Symbol);
        var (success, exception) = newCurrency.AddOrEdit();
        if (success)
        {
            // TODO correct
            // Currencies.AddAndSort(newCurrency, s => s.Symbol);
            // Account.CurrencyFk = newCurrency.Id;

            Log.Information("Account type was successfully added");
            var json = newCurrency.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.MsgBox.Show(CurrencySymbolManagementResources.MessageBoxAddNewCurrencySuccessTitle,
                CurrencySymbolManagementResources.MessageBoxAddNewCurrencySuccessMessage, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.MsgBox.Show(CurrencySymbolManagementResources.MessageBoxAddNewCurrencyErrorTitle,
                CurrencySymbolManagementResources.MessageBoxAddNewCurrencyErrorMessage, MsgBoxImage.Error);
        }
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxDeleteAccountQuestionTitle,
            string.Format(AddEditAccountResources.MessageBoxDeleteAccountQuestionMessage, Account.Name),
            MessageBoxButton.YesNoCancel, MsgBoxImage.Question);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the account \"{AccountToDeleteName}\"", Account.Name);
        var (success, exception) = Account.Delete(true);

        if (success)
        {
            Log.Information("Account was successfully removed");
            MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxDeleteAccountSuccessTitle,
                AddEditAccountResources.MessageBoxDeleteAccountSuccessMessage, MsgBoxImage.Check);

            DeleteAccount = true;
            DialogResult = true;
            Close();
            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxDeleteAccountErrorTitle,
            AddEditAccountResources.MessageBoxDeleteAccountErrorMessage, MsgBoxImage.Error);
    }

    private async void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var error = await CheckIsError();
        if (error) return;

        DialogResult = true;
        Close();
    }

    private async void TextBoxAccountName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            if (AccountViewModel.HasNameChanged &&
                await _accountPresentationValidationService.IsAccountNameAlreadyExist(AccountViewModel.Name))
            {
                MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxErrorAccountNameAlreadyExists, MsgBoxImage.Warning);
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxAddAccountError, MsgBoxImage.Error);
        }
    }

    private void TextBoxStartingBalance_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text.Insert(textBox.SelectionStart, e.Text);

        e.Handled = !txt.IsOnlyDecimal();
    }

    #endregion

    #region Function

    private async Task<bool> CheckIsError()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Creating a new ValidationContext for the Account object to perform data validation.
        // The serviceProvider and items are set to null because they are not required in this context.
        // The ValidationResults list will store any validation errors detected during the process.
        var validationContext = new ValidationContext(Account, serviceProvider: null, items: null);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Using 'var' keeps the code concise and readable, as the type (List<ValidationResult>)
        // is evident from the initialization. The result will still be compatible with any method
        // that expects an ICollection<ValidationResult>, as List<T> implements the ICollection interface.
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(Account, validationContext, validationResults, true);
        if (isValid)
        {
            // var alreadyExiste = await CheckAccountName(Account.Name);
            // if (alreadyExiste)
            // {
            //     MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxErrorAccountNameAlreadyExists, MsgBoxImage.Warning);
            //     return true;
            // }

            if (EnableStartingBalance is false) return false;

            if (string.IsNullOrEmpty(History.Description))
            {
                MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxErrorAccountStartingBalanceDescriptionCannotByEmpty,
                    MsgBoxImage.Warning);
                return true;
            }
        }

        var propertyError = validationResults.First();
        var propertyMemberName = propertyError.MemberNames.First();

        var messageErrorKey = propertyMemberName switch
        {
            nameof(TAccount.Name) => nameof(AddEditAccountResources.MessageBoxButtonValidationAccountNameError),
            nameof(TAccount.AccountTypeFk) => nameof(AddEditAccountResources.MessageBoxButtonValidationAccountTypeFkError),
            nameof(TAccount.CurrencyFk) => nameof(AddEditAccountResources.MessageBoxButtonValidationCurrencyFkError),
            _ => null
        };

        var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
            ? propertyError.ErrorMessage!
            : AddEditAccountResources.ResourceManager.GetString(messageErrorKey)!;

        MsgBox.MsgBox.Show(localizedErrorMessage, MsgBoxImage.Warning);

        return true;
    }

    private async Task FillCollection()
    {
        await Task.WhenAll(
            _categoryPresentationService.GetAllCategoryTypeViewModelAsync().LoadAndSortAsync(CategoryTypes, x => x.Name!),
            _accountPresentationService.GetAllCurrencyViewModelAsync().LoadAndSortAsync(Currencies, x => x.Symbol!),
            _accountPresentationService.GetAllAccountTypeViewModelAsync().LoadAndSortAsync(AccountTypes, x => x.Name!)
        );
    }

    public void SetTAccount(TAccount account)
    {
        account.CopyPropertiesTo(Account);
        EditAccount = true;
    }

    public void SetAccount(AccountViewModel accountViewModel)
    {
        accountViewModel.CopyPropertiesTo(accountViewModel);
        EditAccount = true;
    }

    #endregion
}