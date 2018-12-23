
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;
using TrackManagement;
using Xunit;

namespace SyncTrackLambda.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestTrackProcessor()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new UploadReflexTrackToS3();
            var context = new TestLambdaContext();
            Track testTrack = new Track
            {
                TrackName = "Nik s National Zharikovo",
                SourceTrackUrl = "http://reflex-central.com/tracks/Nik%20s%20National%20Zharikovo.zip",
                SourceThumbnailUrl = "http://reflex-central.com/tracks/Nik%20s%20National%20Zharikovo.jpg",
                Author = "NikDeLion",
                CreationTime = 1458525840
            };

            function.FunctionHandler(JsonConvert.SerializeObject(testTrack), context);
        }

        [Fact]
        public void ReflexCentralMonitor()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new UploadReflexTrackToS3();
            var context = new TestLambdaContext();

            function.ReflexCentralMonitor(context);
        }
    }
}
