using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DarkSlidesTrackListParser
{
    public class TrackListParser
    {
        public TrackListParser()
        {
        }

        public Track[] Parse()
        {
            List<Track> tracks = new List<Track>();
            string html = GetTrackListHtml();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var trackListNodes = doc.DocumentNode.SelectNodes("//div[@id='tracklist']/a");

            foreach (var node in trackListNodes)
            {
                if (node.NodeType == HtmlNodeType.Element && node.Name == "a")
                {
                    string hrefValue = node.Attributes["href"].Value;
                    Track track = new Track
                    {
                        TrackName = node.InnerHtml,
                        DarkSlidesTrackUrl = string.Format("http://ds19.eu/{0}", hrefValue)
                    };
                    
                    string ext = Path.GetExtension(hrefValue);
                    if (ext != ".zip")
                    {
                        track.ErrorInfo += string.Format("Expected .zip file, got {0} file. ", ext);
                        track.Valid = false;
                    }

                    int slot = 0;
                    track.TrackType = GetTrackType(hrefValue, out slot);
                    if(track.TrackType == TrackType.Unknown)
                    {
                        track.ErrorInfo += string.Format("Unknown track type. ", ext);
                        track.Valid = false;
                    }

                    track.SlotNumber = slot;

                    tracks.Add(track);
                }
            }
            return tracks.ToArray();
        }

        private TrackType GetTrackType(string fileName, out int slot)
        {
            TrackType type = TrackType.Unknown;
            slot = 0;
            for (int i = 0; i < 8; ++i)
            {
                string nationalPattern = string.Format("_N{0}.zip", i+1);
                string supercrossPattern = string.Format("_S{0}.zip", i + 1);
                string freeRidePattern = string.Format("_F{0}.zip", i + 1);

                if(fileName.Contains(nationalPattern))
                {
                    type = TrackType.National;
                    slot = i + 1;
                    break;
                }
                else if(fileName.Contains(supercrossPattern))
                {
                    type = TrackType.Supercross;
                    slot = i + 1;
                    break;
                }
                else if (fileName.Contains(freeRidePattern))
                {
                    type = TrackType.FreeRide;
                    slot = i + 1;
                    break;
                }
            }

            return type;
        }

        private string GetTrackListHtml()
        {
            string html = string.Empty;
            using (WebClient client = new WebClient())
            {
                html = client.DownloadString("http://ds19.eu/tracklist.php");
            }
            return html;
        }
    }
}
