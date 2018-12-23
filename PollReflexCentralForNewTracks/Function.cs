using Amazon;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using TrackManagement;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace PollReflexCentralForNewTracks
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void FunctionHandler(int trackId, ILambdaContext context)
        {
            try
            {
                ReflexCentralParser parser = new ReflexCentralParser();

                int NumberOfTracksToEvaluate = 1;
                List<Track> tracks = new List<Track>(NumberOfTracksToEvaluate);
                for (int i = 0; i < NumberOfTracksToEvaluate; ++i)
                {
                    var track = parser.ParseTrack(string.Format("http://reflex-central.com/track_profile.php?track_id={0}", trackId));
                    if(track != null)
                    {
                        tracks.Add(track);
                    }
                    ++trackId;
                }

                if (tracks.Count > 0)
                {
                    var existingTrackNames = HttpUtility.Get<string[]>("https://spptqssmj8.execute-api.us-east-1.amazonaws.com/test/tracknames");
                    var newTracks = tracks.Where(t => existingTrackNames.Any(e => e == t.TrackName) == false).ToArray();
                    foreach (var track in newTracks)
                    {
                        HttpUtility.Post("https://spptqssmj8.execute-api.us-east-1.amazonaws.com/test/uploadtrack", track);
                    }

                }

            }
            catch (Exception e)
            {
                context.Logger.LogLine(e.Message);
            }
        }
    }
}
