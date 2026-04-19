using MyExpenses.Application.Dtos.Categories;
using CategoryTypeViewModel = MyExpenses.Presentation.ViewModels.Expenses.CategoryTypeViewModel;

namespace MyExpenses.Presentation.Mappings.Interfaces;

public interface IExpenseDtoViewModelMapper
{
    /// <summary>
    /// Maps a <see cref="CategoryTypeDto"/> to a <see cref="CategoryTypeViewModel"/>.
    /// </summary>
    /// <param name="src">
    /// The source <see cref="CategoryTypeDto"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="CategoryTypeViewModel"/> containing the mapped data from the provided <see cref="CategoryTypeDto"/>.
    /// </returns>
    public CategoryTypeViewModel MapToViewModel(CategoryTypeDto src);

    /// <summary>
    /// Maps a <see cref="CategoryTypeViewModel"/> to a <see cref="CategoryTypeDto"/>.
    /// </summary>
    /// <param name="src">
    /// The source <see cref="CategoryTypeViewModel"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="CategoryTypeDto"/> containing the mapped data from the provided <see cref="CategoryTypeViewModel"/>.
    /// </returns>
    public CategoryTypeDto MapToDto(CategoryTypeViewModel src);

    /// <summary>
    /// Creates a copy of the provided <see cref="CategoryTypeViewModel"/> instance.
    /// </summary>
    /// <param name="categoryTypeViewModel">
    /// The <see cref="CategoryTypeViewModel"/> instance to be cloned.
    /// </param>
    /// <returns>
    /// A new <see cref="CategoryTypeViewModel"/> instance that is a copy of the provided instance.
    /// </returns>
    public CategoryTypeViewModel Clone(CategoryTypeViewModel categoryTypeViewModel);
}