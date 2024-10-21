namespace MyExpenses.Models.WebApi.Authenticator;

public class Pkce
{
    public required string CodeVerifier { get; init; }
    public required string CodeChallenge { get; init; }
}