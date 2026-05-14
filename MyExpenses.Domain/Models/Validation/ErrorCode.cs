namespace Domain.Models.Validation;

public enum ErrorCode
{
    None,

    AccountRequired,
    AccountTypeRequired,
    ActiveStatusRequired,
    CategoryTypeRequired,
    ColorRequired,
    CurrencyRequired,
    DateRequired,
    DescriptionRequired,
    FrequencyRequired,
    ModePaymentRequired,
    NextDueDateRequired,
    PlaceRequired,
    StartDateRequired,
    RecursiveCountRequired,
    RecursiveTotalRequired,
    ValueRequired,

    CityTooLong,
    CountryTooLong,
    DescriptionTooLong,
    HexadecimalColorCodeTooLong,
    HexadecimalColorCodeRequired,
    HexadecimalColorCodeInvalidFormat,
    HexadecimalColorCodeAlreadyExists,
    LatitudeRequired,
    LongitudeRequired,
    NameTooLong,
    NameRequired,
    NameAlreadyExists,
    NumberTooLong,
    PostalTooLong,
    StreetTooLong,

    InvalidCharacters,

    UnknownError,

    ValidationFailed,
    DatabaseError,
    AccountTypeNotFound,
    CategoryTypeNotFound,
    NotFound,
}