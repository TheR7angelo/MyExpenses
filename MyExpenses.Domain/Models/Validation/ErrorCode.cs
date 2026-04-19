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

    DatabaseError = 100,
    AccountTypeNotFound,
    CategoryTypeNotFound,
    NotFound,
}