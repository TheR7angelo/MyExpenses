using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Expenses;
using Domain.Models.Validation;
using MyExpenses.Presentation.Validations.Attributes;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Expenses;

[DirtyTracking]
public partial class ModePaymentViewModel : ObservableValidator
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.NameRequired, ErrorMessage = "Mode of payment name is required")]
    [MaxLengthWithCode(ModePaymentDomain.MaxNameLength, ErrorCode.NameTooLong, ErrorMessage = "Mode of payment name cannot exceed 55 characters")]
    public partial string? Name { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial bool CanBeDeleted { get; set; } = true;

    public DateTime? DateAdded { get; set; } = DateTime.Now;

    // // [NotMapped]
    // // public EModePayment EModePayment => GetModePayment(Id);
    //
    // // Each ICollection property is initialized to prevent null references
    // // and to ensure the collections are ready for use, even if no data is loaded from the database.
    // // ReSharper disable HeapView.ObjectAllocation.Evident
    // // ReSharper disable PropertyCanBeMadeInitOnly.Global
    // [InverseProperty("ModePaymentFkNavigation")]
    // public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();
    //
    // [InverseProperty("ModePaymentFkNavigation")]
    // public virtual ICollection<TRecursiveExpense> TRecursiveExpenses { get; set; } = new List<TRecursiveExpense>();
    // // ReSharper restore PropertyCanBeMadeInitOnly.Global
    // // ReSharper restore HeapView.ObjectAllocation.Evident
    //
    // /// <summary>
    // /// Retrieves the mode of payment corresponding to the provided identifier.
    // /// </summary>
    // /// <param name="id">The identifier of the mode of payment.</param>
    // /// <returns>The <see cref="EModePayment"/> value corresponding to the given identifier. Returns <see cref="EModePayment.Another"/> if the identifier doesn't match any predefined mode.</returns>
    // public static EModePayment GetModePayment(int? id)
    // {
    //     return id switch
    //     {
    //         1 => EModePayment.BankCard,
    //         2 => EModePayment.BankTransfer,
    //         3 => EModePayment.BankDirectDebit,
    //         4 => EModePayment.BankCheck,
    //         _ => EModePayment.Another
    //     };
    // }

    public IEnumerable<DomainValidationResult> GetErrorCodes()
        => GetErrors().OfType<DomainValidationResult>();
}