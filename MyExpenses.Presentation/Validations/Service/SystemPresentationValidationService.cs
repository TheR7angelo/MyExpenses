using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Validations.Interfaces;

namespace MyExpenses.Presentation.Validations.Service;

public class SystemPresentationValidationService(ISystemService systemService,
    ILogger<SystemPresentationValidationService> logger) : ISystemPresentationValidationService
{
    public async Task<bool> IsColorNameAvailableAsync(string name, CancellationToken cancellationToken = default)
    {
        using var scope = logger.BeginScope("Checking color name availability. Name={Name}",
            name);

        logger.LogInformation("Starting validation for color name availability");

        try
        {
            var alreadyExists = await systemService.IsColorNameAvailableAsync(name, cancellationToken);

            if (!alreadyExists)
            {
                logger.LogInformation("Color name is already used");
                return false;
            }

            logger.LogInformation("Color name is available");
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Validation was canceled");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while checking color name availability");
            throw;
        }
    }

    public async Task<bool> IsColorHexadecimalCodeAvailableAsync(string hexadecimalCode, CancellationToken cancellationToken = default)
    {
        using var scope = logger.BeginScope("Checking color hexadecimal code availability. HexadecimalCode={HexadecimalCode}",
            hexadecimalCode);

        logger.LogInformation("Starting validation for color hexadecimal code availability");

        try
        {
            var alreadyExists = await systemService.IsColorHexadecimalCodeAvailableAsync(hexadecimalCode, cancellationToken);

            if (!alreadyExists)
            {
                logger.LogInformation("Color hexadecimal code is already used");
                return false;
            }

            logger.LogInformation("Color hexadecimal code is available");
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Validation was canceled");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while checking color hexadecimal code availability");
            throw;
        }
    }
}