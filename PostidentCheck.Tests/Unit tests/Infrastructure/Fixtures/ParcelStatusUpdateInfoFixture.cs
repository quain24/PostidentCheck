using System.Collections.Generic;
using System.Linq;
using Postident.Application.Common.Models;
using Postident.Core.Enums;

namespace Postident.Tests.Unit_tests.Infrastructure.Fixtures
{
    public static class ParcelStatusUpdateInfoFixture
    {
        public static IEnumerable<ParcelStatusUpdateInfo> CreateUpdateInfos(string status, Carrier carrier,
            params string[] trackingNumbers)
        {
            List<ParcelStatusUpdateInfo> infos = new(trackingNumbers.Count());

            foreach (var trackingNumber in trackingNumbers)
            {
                infos.Add(new ParcelStatusUpdateInfo { ParcelStatus = status, Carrier = carrier, TrackingNumber = trackingNumber });
            }

            return infos;
        }
    }
}