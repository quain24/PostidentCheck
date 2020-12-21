﻿using System.Collections.Generic;
using Postident.Core.Entities;
using Postident.Core.Enums;

namespace Postident.Tests.Integration_tests.Infrastructure.Common.Fixtures
{
    public static class ParcelFixtureCommon
    {
        public static IEnumerable<DataPackReadModel> GetProperParcelCollection(Carrier carrier, string status = "")
        {
            return new[]
            {
                new DataPackReadModel()
                {
                    CarrierId = ((int) carrier).ToString(),
                    ParcelId = 945537,
                    ParcelStatus = status,
                    TrackingNumber = "00340434161094022102"
                },
                new DataPackReadModel()
                {
                    CarrierId = ((int) carrier).ToString(),
                    ParcelId = 945488,
                    ParcelStatus = status,
                    TrackingNumber = "00340434161094022115"
                },
                new DataPackReadModel()
                {
                    CarrierId = ((int) carrier).ToString(),
                    ParcelId = 945321,
                    ParcelStatus = status,
                    TrackingNumber = "00340434161094027318"
                },
                new DataPackReadModel()
                {
                    CarrierId = ((int) carrier).ToString(),
                    ParcelId = 945111,
                    ParcelStatus = status,
                    TrackingNumber = "00340434161094032954"
                }
            };
        }

        public static DataPackReadModel GetProperParcelWithTrackingNumber(string trackingNumber, string status = "")
        {
            return new DataPackReadModel()
            {
                CarrierId = ((int)Carrier.DHL).ToString(),
                ParcelId = 848211,
                ParcelStatus = status,
                TrackingNumber = trackingNumber
            };
        }
    }
}