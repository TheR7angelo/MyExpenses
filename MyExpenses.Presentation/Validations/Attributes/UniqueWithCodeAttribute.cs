using System.ComponentModel.DataAnnotations;
using Domain.Models.Validation;

namespace MyExpenses.Presentation.Validations.Attributes;

public class UniqueWithCodeAttribute(ErrorCode errorCode) : ValidationAttribute
{

}