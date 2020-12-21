using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Postident.Application.DHL;

namespace Postident.Application.Common.Validators
{
    public class DhlResponseDtoValidator : AbstractValidator<DhlResponseDto>
    {
        private const string KeyErrorMsg = "A {0} cannot be empty - it must correspond with a key of entity that will be updated by its data";
        private const string MainFaultCodeErrorMsg = "A {0} property should never be empty!";

        /// <summary>
        /// This validator validates a state of object, not error codes!
        /// </summary>
        /// <param name="logger"></param>
        public DhlResponseDtoValidator(ILogger<DhlResponseDtoValidator> logger)
        {
            RuleFor(d => d.Key)
                .NotEmpty()
                .OnFailure(d => logger?.LogError(string.Format(KeyErrorMsg, nameof(d.Key))))
                .WithMessage(d => string.Format(KeyErrorMsg, nameof(d.Key)));

            RuleFor(d => d.ErrorCode)
                .NotEmpty()
                .OnFailure(d => logger?.LogError(string.Format(MainFaultCodeErrorMsg, nameof(d.ErrorCode))))
                .WithMessage(d => string.Format(MainFaultCodeErrorMsg, nameof(d.ErrorCode)));
        }

        protected override bool PreValidate(ValidationContext<DhlResponseDto> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", $"Given {nameof(DhlResponseDto)} is NULL."));
                return false;
            }
            return true;
        }
    }
}