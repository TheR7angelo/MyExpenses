using System.Security.Cryptography;
using System.Text;
using MyExpenses.Models.WebApi.Authenticator;

namespace MyExpenses.Share.Core.WebApi;

public static class Utils
{
    public static Pkce GeneratePkceData()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Normal allocation of a 32-byte array for generating the PKCE code verifier.
        // This array is filled with cryptographically secure random bytes.
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
        
        var pkce = new Pkce { CodeChallenge = codeChallenge, CodeVerifier = codeVerifier };

        return pkce;
    }
}