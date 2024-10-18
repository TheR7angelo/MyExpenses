using System.Security.Cryptography;
using System.Text;

namespace MyExpenses.Models.WebApi.Authenticator;

public abstract class AAuthenticator
{
    protected static (string CodeVerifier, string CodeChallenge) GeneratePkceData()
    {
        var codeVerifierBytes = new byte[32];
        RandomNumberGenerator.Fill(codeVerifierBytes);
        var codeVerifier = Convert.ToBase64String(codeVerifierBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        var codeChallengeBytes = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));

        var codeChallenge = Convert.ToBase64String(codeChallengeBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
        return (codeVerifier, codeChallenge);
    }
}