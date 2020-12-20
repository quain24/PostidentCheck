using FluentValidation;
using FluentValidation.Results;
using Postident.Core.Entities;

namespace Postident.Application.Common.Validators
{
    public class DataPackValidator : AbstractValidator<DataPack>
    {
        public DataPackValidator()
        {
            RuleFor(d => d.PostIdent)
                .NotEmpty()
                .Must(s => long.TryParse(s, out var number))
                .WithMessage(d => $"Checked PostIdent ({d.PostIdent}) does not appear to be a number.");

            RuleFor(d => d.CountryCode)
                .NotEmpty()
                .Length(2, 3)
                .WithMessage(d => $"Checked DataPack's (PostIdent {d.PostIdent}) country code length is out of range (must be between 2-3 chars)");

            RuleFor(d => d.ZipCode)
                .NotEmpty()
                .Length(5)
                .WithMessage(d => $"Checked DataPack's (PostIdent {d.PostIdent}) Zip code length is out of range (must be 5 chars precisely)");

            RuleFor(d => d.City)
                .NotEmpty();
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