using FluentValidation;
using FluentValidation.Results;
using Postident.Core.Entities;

namespace Postident.Application.Common.Validators
{
    public class DataPackReadModelValidator : AbstractValidator<DataPackReadModel>
    {
        public DataPackReadModelValidator()
        {
            RuleFor(d => d.Id)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithMessage(d =>
                    $"ID missing! recovered data: {nameof(d.PostIdent)} : {d.PostIdent}, {nameof(d.Carrier)} : {d.Carrier}, {nameof(d.City)} : {d.City}," +
                    $"{nameof(d.CountryCode)} : {d.CountryCode}, {nameof(d.Street)} : {d.Street}, {nameof(d.ZipCode)} : {d.ZipCode}");
            
            When(d => string.IsNullOrWhiteSpace(d.PostIdent) is false, () =>
            {
                RuleFor(d => d.PostIdent)
                    .Must(s => long.TryParse(s, out var number))
                    .WithMessage(d =>
                        $"ID {d.Id}: PostIdent (\"{d.PostIdent}\") is not empty and not a number.");
            });

            RuleFor(d => d.CountryCode)
                .NotEmpty()
                .Length(2, 3)
                .WithMessage(d => $"ID {d.Id}: country code length is out of range (must be between 2-3 chars)");

            RuleFor(d => d.ZipCode)
                .NotEmpty()
                .Length(5)
                .WithMessage(d => $"ID {d.Id}: Zip code length is out of range (must be 5 chars precisely)");

            RuleFor(d => d.DataPackChecked)
                .InclusiveBetween(-1, 1)
                .WithMessage(d => $"ID {d.Id}: 'DataPack checked' value is out of range (must -1, 0 or 1)");

            RuleFor(d => d.City)
                .NotEmpty()
                .WithMessage(d => $"ID {d.Id}: 'City' value is missing");
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