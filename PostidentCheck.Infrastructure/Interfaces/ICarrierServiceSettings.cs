using KeePass.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Interfaces
{
    public interface ICarrierServiceSettings
    {
        /// <summary>
        /// Base address of carriers api, must end in '/'
        /// </summary>
        string BaseAddress { get; set; }

        /// <summary>
        /// Maximum number of queries send by this service to carriers api in one second
        /// </summary>
        int MaxQueriesPerSecond { get; set; }

        /// <summary>
        /// Gets a Login / password pair
        /// </summary>
        /// <returns><see cref="KeePass.Models.Secret"/> containing password / username pair</returns>
        Task<Secret> Secret() => Secret(CancellationToken.None);

        /// <summary>
        /// Gets a Login / password pair
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns><see cref="KeePass.Models.Secret"/> containing password / username pair</returns>
        /// <exception cref="System.OperationCanceledException()"></exception>
        Task<Secret> Secret(CancellationToken ct);
    }
}