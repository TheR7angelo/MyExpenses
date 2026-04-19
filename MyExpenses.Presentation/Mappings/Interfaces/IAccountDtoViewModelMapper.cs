using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Mappings.Interfaces;

public interface IAccountDtoViewModelMapper
{
    /// <summary>
    /// Maps a TotalByAccountDto object to a TotalByAccountViewModel object.
    /// </summary>
    /// <param name="src">The TotalByAccountDto object to be mapped.</param>
    /// <returns>A TotalByAccountViewModel object representing the mapped data.</returns>
    public TotalByAccountViewModel MapToViewModel(TotalByAccountDto src);

    /// <summary>
    /// Maps an AccountDto object to an AccountViewModel object.
    /// </summary>
    /// <param name="src">The AccountDto object to be mapped.</param>
    /// <returns>An AccountViewModel object representing the mapped data.</returns>
    public AccountViewModel MapToViewModel(AccountDto src);

    /// <summary>
    /// Maps an AccountViewModel object to an AccountDto object.
    /// </summary>
    /// <param name="src">The AccountViewModel object to be mapped.</param>
    /// <returns>An AccountDto object representing the mapped data.</returns>
    public AccountDto MapToDto(AccountViewModel src);

    /// <summary>
    /// Maps a CurrencyDto object to a CurrencyViewModel object.
    /// </summary>
    /// <param name="src">The CurrencyDto object to be mapped.</param>
    /// <returns>A CurrencyViewModel object representing the mapped data.</returns>
    public CurrencyViewModel MapToViewModel(CurrencyDto src);

    /// <summary>
    /// Maps a CurrencyViewModel object to a CurrencyDto object.
    /// </summary>
    /// <param name="src">The CurrencyViewModel object containing data to be mapped.</param>
    /// <returns>A CurrencyDto object representing the mapped data.</returns>
    public CurrencyDto MapToDto(CurrencyViewModel src);

    /// <summary>
    /// Maps an AccountTypeDto object to an AccountTypeViewModel object.
    /// </summary>
    /// <param name="src">The AccountTypeDto object to be mapped.</param>
    /// <returns>An AccountTypeViewModel object representing the mapped data.</returns>
    public AccountTypeViewModel MapToViewModel(AccountTypeDto src);

    /// <summary>
    /// Maps an AccountTypeViewModel object to an AccountTypeDto object.
    /// </summary>
    /// <param name="src">The AccountTypeViewModel object to be mapped.</param>
    /// <returns>An AccountTypeDto object representing the mapped data.</returns>
    public AccountTypeDto MapToDto(AccountTypeViewModel src);

    /// <summary>
    /// Creates a deep copy of the given AccountTypeViewModel object.
    /// </summary>
    /// <param name="src">The AccountTypeViewModel object to be cloned.</param>
    /// <returns>A new AccountTypeViewModel object that is a deep copy of the provided source object.</returns>
    public AccountTypeViewModel Clone(AccountTypeViewModel src);
}
