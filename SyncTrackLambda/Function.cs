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
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void FunctionHandler(string input, ILambdaContext context)
        {
            Track track = JsonConvert.DeserializeObject<Track>(input);
            TrackValidator validator = new TrackValidator();
            track = validator.ValidateTrack(track);
            
            if(track.Result == ProcessResult.Success)
            {
                //Store the data and make accessable
            }
            else if(track.Result == ProcessResult.InvalidFileType)
            {
                //Store in staging location for further review
            }
        }
    }
}
