using Postident.Application.DHL;
using Postident.Core.Entities;
using Postident.Core.Enums;
using Postident.Infrastructure.Interfaces;
using SharedExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Postident.Infrastructure.Mappers
{
    public class DhlResponseToWriteModelMapper : IDhlResponseToWriteModelMapper
    {
        public IEnumerable<InfoPackWriteModel> Map(DhlMainResponseDto response)
        {
            _ = response ?? throw new ArgumentNullException(nameof(response));
            if (response.Responses.IsNullOrEmpty())
            {
                return Array.Empty<InfoPackWriteModel>();
            }

            return response.Responses.Select(r =>
                new InfoPackWriteModel
                {
                    CheckStatus = r.IsValid ? InfoPackCheckStatus.Valid : InfoPackCheckStatus.Invalid,
                    Id = r.Key,
                    Message = r
                });
        }

        public IEnumerable<InfoPackWriteModel> Map(IEnumerable<DhlMainResponseDto> responses)
        {
            _ = responses ?? throw new ArgumentNullException(nameof(responses));
            return responses.SelectMany(Map);
        }
    }
}