namespace Domain.Models.Validation;

public enum ErrorCode
{
    None,

    AccountTypeRequired,
    ActiveStatusRequired,
    ColorRequired,
    CurrencyRequired,

    HexadecimalColorCodeTooLong,
    HexadecimalColorCodeRequired,
    NameTooLong,
    NameRequired,
    NameAlreadyExists,

    InvalidCharacters,

    UnknownError,

    DatabaseError = 100,
    AccountTypeNotFound,
    NotFound,
}