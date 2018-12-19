using System;
using System.Linq;

namespace DarkSlidesTrackListParser
{
    class Program
    {
        static string PrintTrackType(TrackType type)
        {
            string value = "Unknown";
            switch(type)
            {
                case TrackType.National:
                    {
                        value = "National";
                        break;
                    }
                case TrackType.Supercross:
                    {
                        value = "Supercross";
                        break;
                    }
                case TrackType.FreeRide:
                    {
                        value = "FreeRide";
                        break;
                    }
            }

            return value;
        }

        static void Main(string[] args)
        {
            TrackListParser parser = new TrackListParser();
            var tracks = parser.Parse();

            var validTracks = tracks.Where(t => t.Valid == true).ToArray();
            Console.WriteLine(string.Format("Listing valid tracks ({0})", validTracks.Length));
            foreach(var track in validTracks)
            {
                Console.WriteLine(string.Format("Name: {0}, Type: {1}, Slot: {2}, Url: {3}", track.TrackName, PrintTrackType(track.TrackType), track.SlotNumber, track.DarkSlidesTrackUrl));
            }

            Console.WriteLine("");
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine("");

            var invlidTracks = tracks.Where(t => t.Valid == false).ToArray();
            Console.WriteLine(string.Format("Listing invlid tracks ({0})", invlidTracks.Length));
            foreach (var track in invlidTracks)
            {
                Console.WriteLine(string.Format("Name: {0}, Type: {1}, Slot: {2}, Reason: {3}", track.TrackName, PrintTrackType(track.TrackType), track.SlotNumber, track.ErrorInfo));
            }
        }
    }
}
