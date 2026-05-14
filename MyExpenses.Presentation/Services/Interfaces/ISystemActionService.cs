using Domain.Models.Validation;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface ISystemActionService
{
    /// <summary>
    /// Creates a new color asynchronously using the specified color view model and cancellation token.
    /// </summary>
    /// <param name="colorViewModel">The view model containing the details of the color to be created.</param>
    /// <param name="cancellationToken">An optional token to observe for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/> object wrapping the created <see cref="ColorViewModel"/>.</returns>
    public Task<Result<ColorViewModel>> CreateColorAsync(ColorViewModel colorViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing color asynchronously using the specified color view model and cancellation token.
    /// </summary>
    /// <param name="colorViewModel">The view model containing the updated details of the color.</param>
    /// <param name="cancellationToken">An optional token to observe for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/> object wrapping the updated <see cref="ColorViewModel"/>.</returns>
    public Task<Result<ColorViewModel>> UpdateColorAsync(ColorViewModel colorViewModel, CancellationToken cancellationToken = default);
}