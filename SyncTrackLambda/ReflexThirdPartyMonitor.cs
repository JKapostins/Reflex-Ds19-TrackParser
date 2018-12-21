using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json;
using TrackManagement;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SyncTrackLambda
{
    public class ReflexThirdPartyMonitor
    {
        public void ParseTrackAndStoreInS3(string input, ILambdaContext context)
        {
            Track track = JsonConvert.DeserializeObject<Track>(input);
            TrackValidator validator = new TrackValidator();
            track = validator.ValidateTrack(track);
            
            if(track.Valid)
            {
                //Store the data and make accessable
            }
            else if(track.Valid)
            {
                //Store in staging location for further review
            }

            track.FixEmptyStrings();
            var success = HttpUtility.Post("https://spptqssmj8.execute-api.us-east-1.amazonaws.com/test/tracks", track);
        }

        public void ReflexCentralMonitor(ILambdaContext context)
        {
            ReflexCentralParser parser = new ReflexCentralParser();
            var tracks = parser.ParseTracks();
            var trackNames = tracks.Select(t => t.TrackName).ToArray();

            //TODO: Get list of track names from our database
        }
    }
}
