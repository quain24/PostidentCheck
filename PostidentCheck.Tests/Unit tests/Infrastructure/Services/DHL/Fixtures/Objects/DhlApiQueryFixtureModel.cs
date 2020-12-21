using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Postident.Core.Entities;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures.Objects
{
    [XmlType("data")]
    public class DhlApiQueryFixtureModel
    {
        private readonly char _pieceCodesSeparator = ';';
        private Func<string, DhlApiQueryFixtureModel> AddCode;

        public DhlApiQueryFixtureModel()
        {
            AddCode = AddFirstParcelTrackingNumber;
        }

        private DhlApiQueryFixtureModel AddFirstParcelTrackingNumber(string pieceCode)
        {
            ParcelTrackingNumbers = pieceCode;
            AddCode = AddAnotherParcelNumber;
            return this;
        }

        private DhlApiQueryFixtureModel AddAnotherParcelNumber(string pieceCode)
        {
            ParcelTrackingNumbers = string.Join(_pieceCodesSeparator, ParcelTrackingNumbers, pieceCode);
            return this;
        }

        public DhlApiQueryFixtureModel AddParcelTrackingNumbers(params string[] pieceCodes)
        {
            foreach (var pieceCode in pieceCodes)
            {
                AddParcelTrackingNumber(pieceCode);
            }

            return this;
        }

        public DhlApiQueryFixtureModel AddParcelTrackingNumbers(IEnumerable<DataPackReadModel> parcels)
        {
            foreach (var parcel in parcels)
            {
                AddParcelTrackingNumber(parcel.TrackingNumber);
            }

            return this;
        }

        public DhlApiQueryFixtureModel AddParcelTrackingNumber(string trackingNumber)
        {
            trackingNumber = trackingNumber?.Trim();
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentOutOfRangeException(nameof(trackingNumber), $"{nameof(trackingNumber)} cannot be empty or null.");
            return AddCode(trackingNumber);
        }

        [XmlAttribute("appname")] public string AppName { get; set; } = string.Empty;
        [XmlAttribute("language-code")] public string LanguageCode { get; set; } = string.Empty;
        [XmlAttribute("password")] public string Password { get; set; } = string.Empty;

        [XmlAttribute("piece-code")]
        public string ParcelTrackingNumbers { get; set; } = string.Empty;

        [XmlAttribute("request")] public string Request { get; set; } = string.Empty;
    }
}