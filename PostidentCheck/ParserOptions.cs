using CommandLine;
using Postident.Core.Enums;
using System;
using System.Collections.Generic;

namespace PostidentCheck
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ParserOptions
    {
        [Option('a', "all", Required = false, Default = true, HelpText = "Checks all available data (default)")]
        public bool CheckAll { get; set; }

        [Option('c', "carrier", Required = false,
            HelpText = "Checks data only for selected carrier names: '--carrier DPD' or '--carrier DPD DHL'. Accept Carrier numerical codes." +
                       " Possible options: DHL, DHL_Sperr, DPD, DPD_PL, HES, Schenker, Schenker_PL, Pick_up, Self_delivery", Separator = ',')]
        public IEnumerable<Carrier> Carriers { get; set; } = Array.Empty<Carrier>();

        [Option('i', "id", Required = false, HelpText = "Checks data only for selected ID's: '--id 1234' or '--id 1234 5678", Separator = ',')]
        public IEnumerable<string> Ids { get; set; } = Array.Empty<string>();
    }
}