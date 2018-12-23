using System;
using System.Linq;
using TrackManagement;

namespace ReflexTrackParser
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Fetching tracks from reflex central...");
                ReflexCentralParser parser = new ReflexCentralParser();
                var tracks = parser.ParseTracks();

                Console.WriteLine("Fetching tracks from our databse...");
                var existingTrackNames = HttpUtility.Get<string[]>("https://spptqssmj8.execute-api.us-east-1.amazonaws.com/test/tracknames");

                Console.WriteLine("Filtering out existing tracks...");
                var newTracks = tracks.Where(t => existingTrackNames.Any(e => e == t.TrackName) == false).ToArray();
                foreach (var track in newTracks)
                {
                    Console.WriteLine(string.Format("Processing {0}...", track.TrackName));
                    HttpUtility.Post("https://spptqssmj8.execute-api.us-east-1.amazonaws.com/test/uploadtrack", track);
                }
                Console.WriteLine("Processing Complete!");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
