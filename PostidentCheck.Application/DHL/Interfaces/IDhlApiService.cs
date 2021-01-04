using Postident.Application.Common.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Application.DHL.Interfaces
{
    public interface IDhlApiService
    {
        /// <summary>
        /// Will attempt to retrieve information from DHL api service about given <see cref="DataPack"/> collection.
        /// </summary>
        /// <param name="dataPacks">Collection of <see cref="DataPack"/> objects to ask API about.</param>
        /// <param name="token">Enables cancellation of this query</param>
        /// <returns>A collection of <see cref="DhlResponseDto"/> objects with data provided by the API.</returns>
        Task<IEnumerable<DhlMainResponseDto>> GetResponsesFromApiAsync(IEnumerable<DataPack> dataPacks, CancellationToken token);
    }
}