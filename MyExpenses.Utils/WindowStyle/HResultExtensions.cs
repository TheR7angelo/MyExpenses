using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace MyExpenses.Utils.WindowStyle;

/// <summary>
/// Provides extension methods for the HResult enum.
/// </summary>
public static class HResultExtensions
{
    /// <summary>
    /// Extension method to check if the HResult represents a failure.
    /// </summary>
    /// <param name="hResult">The HResult value</param>
    /// <returns>True if the HResult represents a failure, otherwise false</returns>
    [Pure]
    private static bool Failed(this HResult hResult)
    {
        return hResult < 0;
    }

    /// <summary>
    /// Extension method to check if the HResult represents a success
    /// </summary>
    /// <param name="hResult">The HResult value</param>
    /// <returns>True if the HResult represents a success, otherwise false</returns>
    [Pure]
    public static bool Succeeded(this HResult hResult)
    {
        return hResult >= HResult.S_OK;
    }

    /// <summary>
    /// Throws an exception if the HResult represents a failure.
    /// </summary>
    /// <param name="hResult">The HResult to check.</param>
    /// <exception cref="Exception">Thrown when the HResult represents a failure.</exception>
    public static void ThrowOnFailure(this HResult hResult)
    {
        if (Failed(hResult))
        {
            throw Marshal.GetExceptionForHR((int) hResult)!;
        }
    }
}