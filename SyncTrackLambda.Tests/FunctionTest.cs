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
            var function = new Function();
            var context = new TestLambdaContext();
            Track testTrack = new Track
            {
                TrackName = "Nik s National Zharikovo",
                TrackUrl = "http://reflex-central.com/tracks/Nik%20s%20National%20Zharikovo.zip",
                ThumbnailUrl = "http://reflex-central.com/tracks/Nik%20s%20National%20Zharikovo.jpg",
                Author = "NikDeLion"
            };

            function.FunctionHandler(JsonConvert.SerializeObject(testTrack), context);
        }
    }
}
