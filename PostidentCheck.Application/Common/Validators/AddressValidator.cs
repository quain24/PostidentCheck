using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Postident.Application.Common.Models;

namespace Postident.Application.Common.Validators
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            When(d => string.IsNullOrWhiteSpace(d.PostIdent) is false, () =>
            {
                RuleFor(d => d.PostIdent)
                    .Must(s => long.TryParse(s, out var number))
                    .WithMessage(d =>
                        $"PostIdent (\"{d.PostIdent}\") is present, but it is not a number.");
            });

            RuleFor(d => d.Street)
                .NotEmpty()
                .WithMessage("Street name is missing");

            RuleFor(d => d.City)
                .NotEmpty()
                .WithMessage("City value is missing");

            RuleFor(d => d.CountryCode)
                .Length(2, 3)
                .WithMessage("Country code length is out of range (must be between 2-3 chars)")
                .Must(s => s.All(char.IsLetter))
                .WithMessage("Country code contains non-letter character(s).");

            When(d => string.IsNullOrWhiteSpace(d.ZipCode) is false, () =>
            {
                RuleFor(d => d.ZipCode)
                    .Length(3, 8)
                    .WithMessage(d => $"Zip code ({d.ZipCode}) length is not valid (3 - 8 chars)");
            });

        }

        protected override bool PreValidate(ValidationContext<Address> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", $"Given {nameof(Address)} was NULL."));
                return false;
            }
            return true;
        }
    }
}