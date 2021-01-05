using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Postident.Core.Entities;
using Postident.Core.Enums;

namespace Postident.Application.Common.Extensions
{
    public static class ILoggerExtensions
    {
        public static void LogWriteModel(this ILogger logger, InfoPackWriteModel writeModel)
        {
            _ = writeModel ?? throw new ArgumentNullException(nameof(writeModel));

            switch (writeModel.CheckStatus)
            {
                case InfoPackCheckStatus.Valid:
                    logger.LogInformation(writeModel.ToString());
                    break;

                case InfoPackCheckStatus.Unchecked:
                    logger.LogWarning(writeModel.ToString());
                    break;

                case InfoPackCheckStatus.Invalid:
                default:
                    logger.LogError(writeModel.ToString());
                    break;
            }
        }

        public static void LogWriteModel(this ILogger logger, IEnumerable<InfoPackWriteModel> writeModels)
        {
            _ = writeModels ?? throw new ArgumentNullException(nameof(writeModels));

            foreach (var entry in writeModels)
            {
                logger.LogWriteModel(entry);
            }
        }
    }
}