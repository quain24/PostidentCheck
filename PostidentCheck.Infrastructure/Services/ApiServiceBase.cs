using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Postident.Application.Common.Extensions;
using Postident.Application.Common.Interfaces;
using Postident.Core.Entities;
using SharedExtensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Services
{
    /// <summary>
    /// Base class for most carrier API services. Acts as a strategy pattern for API communication.
    /// <br/>Provides a way to limit API queries per second (<seealso cref="QueriesPerSecond"/>).
    /// Tries to skip possibly incorrect data and continue to work with proper ones.
    /// </summary>
    /// <typeparam name="TResponseDTO">Type of DTO that implementing class wish to work with</typeparam>
    /// <typeparam name="TQueriedDataType">Type of data object that this service will use when queering API service</typeparam>
    public abstract class ApiServiceBase<TResponseDTO, TQueriedDataType> where TResponseDTO : class where TQueriedDataType : class
    {
        private readonly string _serviceName;
        private readonly ICarrierApiServiceResponseDeserializer<TResponseDTO> _deserializer;
        private readonly ILogger _logger;

        /// <summary>
        /// <inheritdoc cref="ApiServiceBase{TResponseDTO,TQueriedDataType}"/>
        /// </summary>
        /// <param name="serviceName">Name of a carrier to which this API connects - used in logging</param>
        /// <param name="deserializer">Object used to deserialize API responses (<see cref="HttpResponseMessage"/>) into chosen <typeparamref name="TResponseDTO"/> type</param>
        /// <param name="logger">Used to log any errors, optional</param>
        protected ApiServiceBase(string serviceName, ICarrierApiServiceResponseDeserializer<TResponseDTO> deserializer, ILogger logger)
        {
            _serviceName = serviceName;
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer), "Deserializer is required!");
            _logger = logger;
        }

        /// <summary>
        /// When implemented, provides a <see cref="HttpClient"/> to be used with this API service.
        /// </summary>
        protected abstract HttpClient Client { get; }

        /// <summary>
        /// Maximum amount of <see cref="HttpRequestMessage"/> sent in one second
        /// </summary>
        protected abstract int QueriesPerSecond { get; }

        /// <summary>
        /// Queries API service for data taken from <paramref name="dataPacks"/>. Does validate given <paramref name="dataPacks"/> and processes only good ones.
        /// Queries API in set intervals (<seealso cref="QueriesPerSecond"/>). Can be cancelled using with <paramref name="token"/>
        /// </summary>
        /// <param name="dataPacks">Collection of <see cref="DataPackReadModel"/> to ask API about</param>
        /// <param name="token">API query can be cancelled with this token</param>
        /// <returns>Collection of <typeparamref name="TResponseDTO"></typeparamref> from API</returns>
        public async Task<IEnumerable<TResponseDTO>> GetResponsesFromApiAsync(
            IEnumerable<TQueriedDataType> dataPacks, CancellationToken token)
        {
            try
            {
                if (dataPacks.IsNullOrEmpty())
                {
                    _logger?.LogInformation("{0}: No DataPacks for api service to process, exiting now", _serviceName);
                    return Array.Empty<TResponseDTO>();
                }

                if (await AuthorizeClient(token) is false)
                {
                    _logger.LogError("{0}: Could not authorize http client, exiting now", _serviceName);
                    return Array.Empty<TResponseDTO>();
                }

                var requests = await GenerateRequestsFrom(dataPacks, token);
                var responses = await AskRemoteServiceForDataAsync(requests, token);
                return await SerializeResponsesToDtos(responses, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "{0}: Operation has been cancelled or timeout has occurred - this service will now stop and return empty collection.", _serviceName);
                return Array.Empty<TResponseDTO>();
            }
        }

        /// <summary>
        /// When overwritten, provides a method for authentication setup for current <see cref="HttpClient"/>
        /// Typically used for setting up token authentication / basic auth. It is executed on each <see cref="GetResponsesFromApiAsync"/> call
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Enables service to progress further if true. If not, service will stop</returns>
        protected virtual Task<bool> AuthorizeClient(CancellationToken token) => Task.FromResult(true);

        /// <summary>
        /// When overriden, provides a way to generate <see cref="HttpRequestMessage"/> objects for the base class.
        /// <br/>Every API can have different methods, different type of requests, so this method does not have base implementation
        /// </summary>
        /// <param name="dataPacks">Source data to generate <see cref="HttpRequestMessage"/> objects from</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>A collection of request to be send</returns>
        protected abstract Task<IEnumerable<HttpRequestMessage>> GenerateRequestsFrom(IEnumerable<TQueriedDataType> dataPacks, CancellationToken token);

        private async Task<IEnumerable<HttpResponseMessage>> AskRemoteServiceForDataAsync(
            IEnumerable<HttpRequestMessage> requests, CancellationToken token)
        {
            var responses = new ConcurrentBag<HttpResponseMessage>();
            var limiter = new SemaphoreSlim(QueriesPerSecond);
            var context = new Polly.Context().WithLogger(_logger);
            var receivedResponseCounter = 0;

            _logger?.LogInformation("{0}: Starting api call - {1} requests to check.", _serviceName, requests.Count());

            var tasks = requests.Select(async request =>
            {
                request.SetPolicyExecutionContext(context);
                await limiter.WaitAsync(token).ConfigureAwait(false);

                try
                {
                    if (!token.IsCancellationRequested)
                    {
                        var response = await Client.SendAsync(request, token).ConfigureAwait(false);
                        responses.Add(response);
                        _logger?.LogInformation("{0}: Response {1} received - code: {2}", _serviceName, ++receivedResponseCounter, response.StatusCode);
                        await Task.Delay(TimeSpan.FromSeconds(1), token).ConfigureAwait(false);
                    }
                }
                finally
                {
                    limiter.Release();
                }
            });
            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogWarning(
                    "{0} HTTP Client timeout - tried to get {1} responses, got {2}." +
                    " Received responses will be processed\nexception data: {3}"
                    , _serviceName, requests.Count(), (await GetOnlyDeserializableResponses(responses)).Count(), ex.Message);
            }
            catch (BrokenCircuitException)
            {
                _logger?.LogWarning("{0}: Remote api call is now cancelled due to too many consecutive transfer errors" +
                                   " - already received data will be processed.", _serviceName);
            }

            var okResponses = (await GetOnlyDeserializableResponses(responses)).ToList();

            _logger?.LogInformation("{0}: Api call finished, {1}/{2} responses were proper and therefore sent for deserialization"
                , _serviceName, okResponses.Count, requests.Count());

            return okResponses;
        }

        /// <summary>
        /// When overwritten, provides a custom way for deciding which <see cref="HttpResponseMessage"/> are good enough to be send for deserialization into response dto's
        /// </summary>
        /// <param name="responses">Collection of <see cref="HttpResponseMessage"/> from API service</param>
        /// <returns>Collection of <see cref="HttpResponseMessage"/> for deserialization</returns>
        protected virtual Task<IEnumerable<HttpResponseMessage>> GetOnlyDeserializableResponses(IEnumerable<HttpResponseMessage> responses) =>
            Task.FromResult(responses.Where(r => r.IsSuccessStatusCode));

        /// <summary>
        /// Method used to facilitate deserialization of raw API response into corresponding <typeparamref name="TResponseDTO"/> type object,
        /// uses given <see cref="ICarrierApiServiceResponseDeserializer{TDeserializedDtoType}"/> passed to this objects instance
        /// Can be overriden.
        /// </summary>
        /// <param name="httpResponses">Collection of <see cref="HttpResponseMessage"/> to generate <see cref="TResponseDTO"/> objects from</param>
        /// <param name="token">Can be used to cancel this operation</param>
        /// <returns>Collection of deserialized objects</returns>
        protected virtual async Task<IEnumerable<TResponseDTO>> SerializeResponsesToDtos(IEnumerable<HttpResponseMessage> httpResponses, CancellationToken token)
        {
            var dtos = await Task.WhenAll(httpResponses.Select(async response => await _deserializer.Deserialize(response))).ConfigureAwait(false);

            _logger?.LogInformation("{0}: Deserialized {1}/{2} responses successfully", _serviceName, dtos.Count(d => d is not null), httpResponses.Count());
            return dtos.Where(d => d is not null);
        }
    }
}