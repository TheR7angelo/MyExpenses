namespace Domain.Models.Validation;

public enum ErrorCode
{
    None,

    AccountTypeRequired,
    ActiveStatusRequired,
    CurrencyRequired,

    NameTooLong,
    NameRequired,
    NameAlreadyExists,

    InvalidCharacters,

    UnknownError,

    DatabaseError = 100,
    AccountTypeNotFound,
    NotFound,
}