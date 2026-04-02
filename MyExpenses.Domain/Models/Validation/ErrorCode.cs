namespace Domain.Models.Validation;

public enum ErrorCode
{
    None,
    NameTooLong,
    NameRequired,
    NameAlreadyExists,
    InvalidCharacters,
    UnknownError,

    DatabaseError = 100,
    AccountTypeNotFound
}