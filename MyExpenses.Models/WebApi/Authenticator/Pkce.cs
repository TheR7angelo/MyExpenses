namespace MyExpenses.Models.WebApi.Authenticator;

public readonly struct Pkce
{
    public required string CodeVerifier { get; init; }
    public required string CodeChallenge { get; init; }
}