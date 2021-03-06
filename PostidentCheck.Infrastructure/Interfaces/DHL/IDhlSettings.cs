using KeePass.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Interfaces.DHL
{
    public interface IDhlSettings : ICarrierServiceSettings
    {
        /// <summary>
        /// Additional username / password pair for dhl service used in xml requests
        /// </summary>
        /// <returns><see cref="Secret"/> containing username and password for dhl xml request generation</returns>
        Task<Secret> XmlSecret() => XmlSecret(CancellationToken.None);

        /// <summary>
        /// Additional username / password pair for dhl service used in xml requests
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns><see cref="Secret"/> containing username and password for dhl xml request generation</returns>
        /// <exception cref="System.OperationCanceledException()"></exception>
        Task<Secret> XmlSecret(CancellationToken ct);

        /// <summary>
        /// How many parcel tracking numbers ca be sent to api in a single request - maximum 20
        /// </summary>
        int MaxValidationsInQuery { get; set; }
    }
}