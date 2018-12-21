using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using SyncTrackLambda;
using TrackManagement;
using Newtonsoft.Json;

namespace SyncTrackLambda.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestTrackProcessor()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new ReflexThirdPartyMonitor();
            var context = new TestLambdaContext();
            Track testTrack = new Track
            {
                TrackName = "Nik s National Zharikovo",
                SourceTrackUrl = "http://reflex-central.com/tracks/Nik%20s%20National%20Zharikovo.zip",
                SourceThumbnailUrl = "http://reflex-central.com/tracks/Nik%20s%20National%20Zharikovo.jpg",
                Author = "NikDeLion",
                CreationTime = 1458525840
            };

            function.ParseTrackAndStoreInS3(JsonConvert.SerializeObject(testTrack), context);
        }

        [Fact]
        public void ReflexCentralMonitor()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new ReflexThirdPartyMonitor();
            var context = new TestLambdaContext();

            function.ReflexCentralMonitor(context);
        }
    }
}
