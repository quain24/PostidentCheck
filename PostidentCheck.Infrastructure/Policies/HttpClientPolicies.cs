using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Postident.Application.Common.Extensions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Policies
{
    // Usage of string.Format and {0} parameters instead of $"{param} text" for logging frameworks compatibility.
    public static class HttpClientPolicies
    {
        /// <summary>
        /// DHL http circuit breaker policy for <see cref="HttpClient"/>, excludes <see cref="HttpStatusCode.InternalServerError"/>.
        /// <para>If this policy will be hit without any good call in between for <paramref name="allowedConsecutiveFailures"/> times, it will throw a <see cref="BrokenCircuitException"/><br/>
        /// This policy does not reset - this is by design</para>
        /// <para>Supports logging of warnings if <see cref="ILogger"/> was supplied to this instance by <see cref="Context"/></para>
        /// </summary>
        /// <param name="serviceName">Name of service - used in logging</param>
        /// <param name="allowedConsecutiveFailures">Amount of allowed one after another bad calls before breaking</param>
        /// <exception cref="BrokenCircuitException">Will be thrown when this policy "breaks"</exception>
        /// <returns>Policy instance to be used with <see cref="HttpClient"/></returns>
        public static IAsyncPolicy<HttpResponseMessage> CircuitBreakerAsyncOneTimePolicy(string serviceName, int allowedConsecutiveFailures)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(msg => !msg.IsSuccessStatusCode && msg.StatusCode != HttpStatusCode.InternalServerError)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: allowedConsecutiveFailures,
                    // No backing off, if its broken, it stays broken
                    durationOfBreak: TimeSpan.FromSeconds(int.MaxValue),
                    onBreak: (result, span, context) =>
                    {
                        context.GetLogger()?.LogWarning("{0}: Too many consecutive service failures, API calls will now terminate.", serviceName);
                    },
                    onReset: context =>
                    {
                        context.GetLogger()?.LogInformation("{0}: http call now allowed to proceed.", serviceName);
                    }
                );
        }

        /// <summary>
        /// Standard DHL Wait and retry policy for <see cref="HttpClient"/>, excludes <see cref="HttpStatusCode.InternalServerError"/>.
        /// <para>Will wait progressively longer with each try (current retry number in seconds)</para>
        /// <para>Supports logging of warnings if <see cref="ILogger"/> was supplied to this instance by <see cref="Context"/></para>
        /// </summary>
        /// <param name="serviceName">Name of service - used in logging</param>
        /// <param name="retryAttempts">Amount of retries to perform</param>
        /// <returns>Policy instance to be used with <see cref="HttpClient"/></returns>
        public static IAsyncPolicy<HttpResponseMessage> WaitAndRetryAsyncPolicy(string serviceName, int retryAttempts)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(msg => !msg.IsSuccessStatusCode && msg.StatusCode != HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(retryAttempts, retryAttempt => TimeSpan.FromSeconds(1 * retryAttempt),
                    (response, _, retryNumber, context) =>
                    {
                        var message = string.Format("{0}: request failed, retrying: {1}/{2}.", serviceName, retryNumber, retryAttempts);
                        if (response?.Result?.StatusCode is not null)
                        {
                            message += string.Format(" Response code: {0} ({1}).", response.Result.StatusCode, (int)response.Result.StatusCode);
                            if (retryNumber == retryAttempts)
                                message += string.Format("\nFailed request url: {0}", response?.Result?.RequestMessage?.RequestUri);
                        }

                        if (response?.Exception is not null)
                        {
                            message += string.Format(" Exception type: {0}. Exception Message: {1}", response.Exception.GetType().Name, response.Exception.Message);
                        }
                        context.GetLogger()?.LogWarning(message);
                    });
        }

        /// <summary>
        /// This policy will generate a "dummy" <see cref="HttpResponseMessage"/> with empty content and <see cref="HttpResponseMessage.StatusCode"/> == <see cref="HttpStatusCode.NotFound"/>
        /// if a <see cref="HttpRequestException"/> was catch.
        /// <para>Supports logging of warnings if <see cref="ILogger"/> was supplied to this instance by <see cref="Context"/></para>
        /// </summary>
        /// <param name="serviceName">Name of service - used in logging</param>
        /// <returns>Policy instance to be used with <see cref="HttpClient"/></returns>
        public static IAsyncPolicy<HttpResponseMessage> FallbackForHttpRequestException(string serviceName)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .FallbackAsync<HttpResponseMessage>(
                    fallbackAction: (result, ctx, token) =>
                    Task.FromResult(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Content = new StringContent(string.Empty)
                    }),
                    onFallbackAsync: (response, context) =>
                    {
                        context.GetLogger()?.LogWarning(response.Exception, "{0}: {1}" +
                                                        " occurred, empty \"Not found\" response returned." +
                                                        "\n{2}", serviceName, response.Exception.GetType().Name, response?.Exception?.InnerException?.Message);
                        return Task.CompletedTask;
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> FallbackForCircuitBreaker()
        {
            return Policy<HttpResponseMessage>
                .Handle<BrokenCircuitException>()
                .FallbackAsync(
                    fallbackAction: async (context, token) =>
                    {
                        var count = (int)context["FallbackCount"];
                        context["FallbackCount"] = ++count;
                        return await Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, Content = new StringContent(string.Empty) });
                    },
                    onFallbackAsync: (result, context) =>
                    {
                        var count = (int)context["FallbackCount"];
                        if (count == 2)
                        {
                            context.GetLogger()?.LogError("No more fallback");
                            throw new BrokenCircuitException("", result.Exception);
                        }
                        context.GetLogger()?.LogError("fallback!");
                        return Task.CompletedTask;
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> FallbackForTimeout(ILogger logger, string carrierName)
        {
            return Policy<HttpResponseMessage>
                .Handle<OperationCanceledException>()
                .FallbackAsync(_ =>
                {
                    var output = new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, Content = new StringContent(string.Empty) };
                    return Task.FromResult(output);
                }, (response) =>
                {
                    logger.LogWarning("{0}: Request timeout occurred, empty \"Not found\" response returned {1}", carrierName, response?.Result?.StatusCode);
                    return Task.FromResult(false);
                });
        }
    }
}