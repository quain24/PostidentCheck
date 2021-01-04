using System;
using CommandLine;
using Postident.Core.Enums;
using System.Collections.Generic;

namespace PostidentCheck.Services
{
    public class ParserOptions
    {
        [Option('a', "All", Required = false, Default = true, HelpText = "Checks all available data (default)")]
        public bool CheckAll { get; set; }

        [Option('c', "Carrier", Required = false, HelpText = "Checks data only for selected carrier names, separated by coma: '--Carrier DPD' or '--Carrier \"DPD, DHL\". Possible options: DHL, DHL_Sperr, DPD, DPD_PL, HES, Schenker, Pick_up, Self_delivery", Separator = ',')]
        public IEnumerable<Carrier> Carriers { get; set; } = Array.Empty<Carrier>();

        [Option('i', "Id", Required = false, HelpText = "Checks data only for selected ID's, separated by coma: '--Id 1234' or '--Id \"1234, 5678\"", Separator = ',')]
        public IEnumerable<string> Ids { get; set; } = Array.Empty<string>();
    }
}