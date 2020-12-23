using FluentValidation;
using FluentValidation.Results;
using Postident.Core.Entities;

namespace Postident.Application.Common.Validators
{
    public class DataPackReadModelValidator : AbstractValidator<DataPackReadModel>
    {
        public DataPackReadModelValidator()
        {
            RuleFor(d => d.PostIdent)
                .NotEmpty()
                .Must(s => long.TryParse(s, out var number))
                .WithMessage(d => $"Checked DataPack's (ID {d.Id}) PostIdent (\"{d.PostIdent}\") does not appear to be a number.");

            RuleFor(d => d.CountryCode)
                .NotEmpty()
                .Length(2, 3)
                .WithMessage(d => $"Checked DataPack's (ID {d.Id}) country code length is out of range (must be between 2-3 chars)");

            RuleFor(d => d.ZipCode)
                .NotEmpty()
                .Length(5)
                .WithMessage(d => $"Checked DataPack's (ID {d.Id}) Zip code length is out of range (must be 5 chars precisely)");

            RuleFor(d => d.DataPackChecked)
                .InclusiveBetween(-1, 1)
                .WithMessage(d => $"Checked DataPack's (ID {d.Id}) 'DataPack checked' value is out of range (must -1, 0 or 1)");

            RuleFor(d => d.City)
                .NotEmpty();

            RuleFor(d => d.Id)
                .NotEmpty();
        }

        protected override bool PreValidate(ValidationContext<DataPackReadModel> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", $"Given {nameof(DataPackReadModel)} was NULL."));
                return false;
            }
            return true;
        }
    }
}