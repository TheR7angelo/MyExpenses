namespace MyExpenses.Presentation.Validations.Interfaces;

public interface ISystemPresentationValidationService
{
    /// <summary>
    /// Determines whether a given color name is available for use.
    /// </summary>
    /// <param name="name">The name of the color to validate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the color name is available.</returns>
    public Task<bool> IsColorNameAvailableAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a given hexadecimal code for a color is available for use.
    /// </summary>
    /// <param name="hexadecimalCode">The hexadecimal code of the color to validate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the hexadecimal code is available.</returns>
    public Task<bool> IsColorHexadecimalCodeAvailableAsync(string hexadecimalCode, CancellationToken cancellationToken = default);
}