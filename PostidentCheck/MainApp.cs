using CommandLine;
using MediatR;
using Microsoft.Extensions.Logging;
using Postident.Application.DHL.Commands;
using Postident.Infrastructure.Common;
using PostidentCheck.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Postident.Core.Enums;

namespace PostidentCheck
{
    public class MainApp
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MainApp> _logger;

        public MainApp(IMediator mediator, ILogger<MainApp> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Run(string[] args)
        {
            await using var writer = new Utf8StringWriter();
            var parser = CreateParser(writer);

            var result = await parser
                .ParseArguments<ParserOptions>(args)
                .WithParsedAsync(async o =>
                {
                    if (o.Carriers.Any())
                    {
                        var carriers = string.Join(", ", o.Carriers);
                        _logger?.LogInformation("Checking data that belongs to selected carriers: --Carrier {0}", carriers);
                        await _mediator.Send(new ValidateDataByCarrierCommand(o.Carriers));
                    }
                    else if (o.Ids.Any())
                    {
                        var ids = string.Join(", ", o.Ids);
                        _logger?.LogInformation("Checking data with selected ID's: --Id {0}", ids);
                        await _mediator.Send(new ValidateDataByIdCommand(o.Ids));
                    }
                    else if (o.CheckAll)
                    {
                        _logger?.LogInformation("Checking all data - default.");
                        await _mediator.Send(new ValidateAllDataCommand());
                    }
                }).ConfigureAwait(false);

            LogInformation(result, writer);
            Console.WriteLine("FINISHED");
        }

        private static Parser CreateParser(TextWriter writer)
        {
            return new(with =>
           {
               with.IgnoreUnknownArguments = false;
               with.CaseInsensitiveEnumValues = true;
               with.HelpWriter = writer;
           });
        }

        private void LogInformation<TOptions>(ParserResult<TOptions> results, TextWriter writer) where TOptions : class
        {
            results.WithNotParsed(errors =>
            {
                var message = writer.ToString();
                if (errors.Any(e => e.Tag != ErrorType.HelpRequestedError && e.Tag != ErrorType.VersionRequestedError))
                {
                    _logger?.LogError(message);
                }
                else
                {
                    _logger?.LogInformation(message);
                }
            });
        }
    }
}