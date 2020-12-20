using System.Net.Http;
using System.Threading.Tasks;

namespace Postident.Application.Common.Interfaces
{
    public interface ICarrierApiServiceResponseDeserializer<TDeserializedDtoType> where TDeserializedDtoType : class
    {
        /// <summary>
        /// Deserializes Carrier API service <see cref="HttpResponseMessage"/> into <typeparamref name="TDeserializedDtoType"/> object
        /// </summary>
        /// <param name="apiMessage">A <see cref="HttpResponseMessage"/> from one of carrier API</param>
        /// <returns>Deserialized <typeparamref name="TDeserializedDtoType"/> object</returns>
        Task<TDeserializedDtoType> Deserialize(HttpResponseMessage apiMessage);
    }
}