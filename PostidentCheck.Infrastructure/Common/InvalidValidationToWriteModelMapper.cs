using FluentValidation.Results;
using Postident.Application.Common.Extensions;
using Postident.Core.Entities;
using Postident.Core.Enums;
using Postident.Infrastructure.Interfaces;
using System;

namespace Postident.Infrastructure.Common
{
    public class InvalidValidationToWriteModelMapper : IInvalidValidationToWriteModelMapper<InfoPackWriteModel>
    {
        public InfoPackWriteModel MapInvalidResult(ValidationResult result, string id)
        {
            if (result.IsValid)
            {
                throw new ArgumentOutOfRangeException(nameof(result),
                    "Tried to map a VALID result with method that only maps invalid ones");
            }

            return new InfoPackWriteModel
            {
                CheckStatus = InfoPackCheckStatus.Invalid,
                Id = id,
                Message = "Offline validation: " + result.CombinedErrors()
            };
        }
    }
}