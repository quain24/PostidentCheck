using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Postident.Application.Common.Models;
using Postident.Core.Entities;

namespace Postident.Application.Common.Interfaces
{
    public interface IValidationService
    {
        /// <summary>
        /// Performs offline and online validation of given <paramref name="dataPacks"/>
        /// </summary>
        /// <param name="dataPacks">Collection to be validated</param>
        /// <param name="token">Cancels running validation</param>
        /// <returns>A collection of <see cref="InfoPackWriteModel"/></returns>
        Task<IEnumerable<InfoPackWriteModel>> Validate(IEnumerable<DataPack> dataPacks, CancellationToken token);
    }
}