using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;

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
                        track.ErrorInfo += string.Format("Expected .zip file, got {0} file; ", ext);
                        track.Valid = false;
                    }
                    else
                    {
                        track = PeekZipFile(track.DarkSlidesTrackUrl, track);
                    }

                    tracks.Add(track);
                }
            }
            return tracks.ToArray();
        }

        private TrackType GetTrackType(string fileName)
        {
            TrackType type = TrackType.Unknown;
            for (int i = 0; i < 8; ++i)
            {
                string nationalPattern = string.Format("Beta_Nat_Track", i+1);
                string supercrossPattern = string.Format("Beta_Sx_Track", i + 1);
                string freeRidePattern = string.Format("Beta_Track", i + 1);

                if(fileName.Contains(nationalPattern))
                {
                    type = TrackType.National;
                    break;
                }
                else if(fileName.Contains(supercrossPattern))
                {
                    type = TrackType.Supercross;
                    break;
                }
                else if (fileName.Contains(freeRidePattern))
                {
                    type = TrackType.FreeRide;
                    break;
                }
            }

            return type;
        }

        private int GetSlot(string fileName)
        {
            int slot = 0;
            const int MaxSlots = 7;
            for(int i = 0; i < MaxSlots; ++i)
            {
                string format = string.Format("Slot_{0}.dx9", i + 1);
                if(fileName.Contains(format))
                {
                    slot = i + 1;
                    break;
                }
            }

            return slot;
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

        private Track PeekZipFile(string url, Track track)
        {
            using (WebClient client = new WebClient())
            {
                using (Stream memoryStream = new MemoryStream(client.DownloadData(url)))
                {
                    using (ZipArchive archive = new ZipArchive(memoryStream))
                    {
                        track = ValidateZipArchive(archive, track);
                    }
                }
            }
            
            //Force the garbage collector to run. We care more about memory than performance when this is running in the cloud.
            GC.Collect();
            GC.WaitForPendingFinalizers();

            return track;
        }

        private Track ValidateZipArchive(ZipArchive archive, Track track)
        {
            int databaseCount = 0;
            int levelCount = 0;
            int packageCount = 0;
            int sceneCount = 0;

            string databaseExt = ".dx9.database";
            string levelExt = ".dx9.level";
            string packageExt = ".dx9.package";
            string sceneExt = ".dx9.scene";

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if(track.TrackType == TrackType.Unknown)
                {
                    track.TrackType = GetTrackType(entry.FullName);
                    track.SlotNumber = GetSlot(entry.FullName);
                }

                if (entry.FullName.EndsWith(databaseExt, StringComparison.OrdinalIgnoreCase))
                {
                    ++databaseCount;
                }
                else if (entry.FullName.EndsWith(levelExt, StringComparison.OrdinalIgnoreCase))
                {
                    ++levelCount;
                }
                else if (entry.FullName.EndsWith(packageExt, StringComparison.OrdinalIgnoreCase))
                {
                    ++packageCount;
                }
                else if (entry.FullName.EndsWith(sceneExt, StringComparison.OrdinalIgnoreCase))
                {
                    ++sceneCount;
                }
            }

            if (track.TrackType == TrackType.Unknown)
            {
                track.ErrorInfo += "Unknown track type; ";
                track.Valid = false;
            }

            if (track.SlotNumber == 0)
            {
                track.ErrorInfo += "Unknown slot; ";
                track.Valid = false;
            }

            track = CheckForRequiredFiles(track, databaseExt, databaseCount);
            track = CheckForRequiredFiles(track, levelExt, levelCount);
            track = CheckForRequiredFiles(track, packageExt, packageCount);
            track = CheckForRequiredFiles(track, sceneExt, sceneCount);

            return track;
        }

        private Track CheckForRequiredFiles(Track track, string fileExtention, int timesFileAppearsInZip)
        {
            if (timesFileAppearsInZip == 0)
            {
                track.ErrorInfo += string.Format("Missing *{0} file; ", fileExtention);
                track.Valid = false;
            }
            else if (timesFileAppearsInZip > 1)
            {
                track.ErrorInfo += string.Format("Expecting 1 *{0} file but got {1}. Please only upload one track per zip file; ", fileExtention, timesFileAppearsInZip);
                track.Valid = false;
            }
            return track;
        }
    }
}
