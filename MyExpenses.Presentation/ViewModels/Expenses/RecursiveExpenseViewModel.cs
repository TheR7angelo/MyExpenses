using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Expenses;
using Domain.Models.Validation;
using MyExpenses.Presentation.Validations.Attributes;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Systems;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Expenses;

[DirtyTracking]
public partial class RecursiveExpenseViewModel : ObservableValidator
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.AccountRequired, ErrorMessage = "Account is required")]
    public partial AccountViewModel? AccountViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.DescriptionRequired, ErrorMessage = "Description is required")]
    [MaxLengthWithCode(RecursiveExpenseDomain.MaxDescriptionLength, ErrorCode.DescriptionTooLong, ErrorMessage = "Description must be less than 255 characters")]
    public partial string? Description { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [MaxLengthWithCode(RecursiveExpenseDomain.MaxNoteLength, ErrorCode.DescriptionTooLong, ErrorMessage = "Note must be less than 255 characters")]
    public partial string? Note { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.CategoryTypeRequired, ErrorMessage = "Category type is required")]
    public partial CategoryTypeViewModel? CategoryTypeViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.ModePaymentRequired, ErrorMessage = "Mode payment is required")]
    public partial ModePaymentViewModel? ModePaymentViewModel { get; set; }

    // [NotMapped]
    // public EModePayment EModePaymentFk => TModePayment.GetModePayment(ModePaymentFk);

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.ValueRequired, ErrorMessage = "Value is required")]
    public partial double? Value { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.PlaceRequired, ErrorMessage = "Place is required")]
    public partial PlaceViewModel? PlaceViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.StartDateRequired, ErrorMessage = "Start date is required")]
    public partial DateOnly? StartDate { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.RecursiveTotalRequired, ErrorMessage = "Recursive total is required")]
    public partial int? RecursiveTotal { get; set; } = 0;

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.RecursiveCountRequired, ErrorMessage = "Recursive count is required")]
    public partial int? RecursiveCount { get; set; } = 0;

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.FrequencyRequired, ErrorMessage = "Frequency is required")]
    public partial RecursiveFrequencyViewModel? RecursiveFrequencyViewModel { get; set; }

    // [NotMapped]
    // public ERecursiveFrequency ERecursiveFrequency
    // {
    //     get => (ERecursiveFrequency)FrequencyFk;
    //     init => FrequencyFk = (int)value;
    // }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.NextDueDateRequired, ErrorMessage = "Next due date is required")]
    public partial DateOnly? NextDueDate { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial bool IsActive { get; set; } = true;

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial bool ForceDeactivate { get; set; } = false;

    public DateTime? DateAdded { get; set; }

    public DateTime? LastUpdated { get; set; }
}