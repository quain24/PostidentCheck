using FluentValidation;
using FluentValidation.Results;
using Postident.Application.Common.Models;

namespace Postident.Application.Common.Validators
{
    public class DataPackValidator : AbstractValidator<DataPack>
    {
        public DataPackValidator(IValidator<Address> addressValidator)
        {
            RuleFor(d => d.Id)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithErrorCode("id_missing")
                .WithMessage(d => "ID missing!");

            RuleFor(d => d.DataPackChecked)
                .IsInEnum()
                .WithMessage(d => $"ID {d.Id}: 'DataPack checked' value is out of range");

            RuleFor(d => d.Carrier)
                .NotEmpty()
                .WithMessage(d => $"ID {d.Id}: 'Carrier' value is missing")
                .IsInEnum()
                .WithMessage(d => $"ID {d.Id}: 'Carrier' value ({d.Carrier}) is out of range of enum");

            RuleFor(d => d.Address)
                .NotNull()
                .WithMessage("Address is missing")
                .SetValidator(addressValidator);
        }

        protected override bool PreValidate(ValidationContext<DataPack> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", $"Given {nameof(DataPack)} was NULL."));
                return false;
            }
            return true;
        }
    }
}