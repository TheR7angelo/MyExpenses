using Domain.Models.Validation;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface ISystemPresentationService
{
    /// <summary>
    /// Retrieves a random color view model asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="Result"/>
    /// indicating the operation's success or failure.</return>
    public Task<ColorViewModel> GetRandomColorViewModel(CancellationToken cancellationToken = default);
}