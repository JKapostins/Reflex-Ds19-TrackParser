using Amazon;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using TrackManagement;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SyncTrackLambda
{
    public class UploadReflexTrackToS3
    {
        public void FunctionHandler(string input, ILambdaContext context)
        {
            try
            {
                Track track = JsonConvert.DeserializeObject<Track>(input);
                TrackValidator validator = new TrackValidator();
                MemoryStream zipStream = new MemoryStream();
                ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true);

                context.Logger.LogLine(string.Format("Beginning to process {0}", track.TrackName));
                track = validator.ValidateTrack(track, (e) =>
                {
                    var destFile = zipArchive.CreateEntry(e.FullName);

                    using (var destStream = destFile.Open())
                    using (var srcStream = e.Open())
                    {
                        var task = srcStream.CopyToAsync(destStream);
                        task.Wait();
                    }
                });
                zipArchive.Dispose();
                zipStream.Position = 0;

                string folderName = Path.GetFileNameWithoutExtension(track.SourceTrackUrl.Replace("%20", " "));
                string imageFileName = Path.GetFileName(track.SourceThumbnailUrl).Replace("%20", " ");
                string trackFileName = Path.GetFileName(track.SourceTrackUrl).Replace("%20", " ");
                string bucketName = track.Valid ? "reflextracks" : "invalidreflextracks";

                using (WebClient client = new WebClient())
                {
                    using (Stream thumbNailStream = new MemoryStream(client.DownloadData(track.SourceThumbnailUrl)))
                    {
                        var uploadTask = AwsS3Utility.UploadFileAsync(thumbNailStream, string.Format("{0}/{1}", bucketName, folderName), imageFileName, RegionEndpoint.USEast1);
                        uploadTask.Wait();
                    }

                    if (track.Valid)
                    {
                        var uploadTask = AwsS3Utility.UploadFileAsync(zipStream, string.Format("{0}/{1}", bucketName, folderName), trackFileName, RegionEndpoint.USEast1);
                        uploadTask.Wait();
                    }
                    else
                    {
                        using (Stream invalidStream = new MemoryStream(client.DownloadData(track.SourceTrackUrl)))
                        {
                            var uploadTask = AwsS3Utility.UploadFileAsync(invalidStream, string.Format("{0}/{1}", bucketName, folderName), trackFileName, RegionEndpoint.USEast1);
                            uploadTask.Wait();
                        }
                    }
                }

                track.FixEmptyStrings();
                var success = HttpUtility.Post("https://spptqssmj8.execute-api.us-east-1.amazonaws.com/test/tracks", track);

                context.Logger.LogLine(string.Format("Processing {0} is complete!", track.TrackName));
            }
            catch (Exception e)
            {
                context.Logger.LogLine(e.Message);
            }
        }

        public void ReflexCentralMonitor(ILambdaContext context)
        {
            try
            {
                var existingTrackNames = HttpUtility.Get<string[]>("https://spptqssmj8.execute-api.us-east-1.amazonaws.com/test/tracknames");

                ReflexCentralParser parser = new ReflexCentralParser();
                var tracks = parser.ParseTracks();
                var newTracks = tracks.Where(t => existingTrackNames.Any(e => e == t.TrackName) == false).ToArray();

                foreach(var track in newTracks)
                {
                    //GNARLY_TODO: call ParseTrackAndStoreInS3 to process new tracks
                }

            }
            catch(Exception e)
            {
                context.Logger.LogLine(e.Message);
            }
        }
    }
}
