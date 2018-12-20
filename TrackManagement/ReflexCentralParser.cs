using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Linq;
using System.Diagnostics;

namespace TrackManagement
{
    public class ReflexCentralParser : HtmlParser
    {
        public ReflexCentralParser()
        {
        }

        public override Track[] ParseTracks()
        {
            List<Track> tracks = new List<Track>();

            int invalidTrackCount = 0;
            const int InvalidThreshold = 100;
            for(int i = 1; ; ++i)
            {
                //run through all track id's until we hit one without a track name. We assume if there is no track names, the are no more tracks to parse
                var profileUrl = string.Format("http://reflex-central.com/track_profile.php?track_id={0}", i);
                var profileHtml = GetHtml(profileUrl);

                var reflexProfileDoc = new HtmlDocument();
                reflexProfileDoc.LoadHtml(profileHtml);


                var trackNameNode = reflexProfileDoc.DocumentNode.SelectNodes("//*[@id='maincontent']/font[1]/b").SingleOrDefault();

                if(trackNameNode == null)
                {
                    ++invalidTrackCount;
                    //There are some false positives in the id list. To avoid getting stopped before we really got all the tracks,
                    //we look for multiple invalid tracks in a row.
                    if (invalidTrackCount > InvalidThreshold)
                    {
                        break;
                    }
                }

                invalidTrackCount = 0;
                var trackName = trackNameNode.InnerHtml;
                var imageNode = reflexProfileDoc.DocumentNode.SelectNodes("//*[@id='track_preview_frame']/img").SingleOrDefault();
                var imageUrl = imageNode != null ? string.Format("http://reflex-central.com/{0}", imageNode.Attributes["src"].Value) : string.Empty;
                var authorNode = reflexProfileDoc.DocumentNode.SelectNodes("//*[@id='maincontent']/font[3]/b/a").SingleOrDefault();
                var author = authorNode != null ? authorNode.InnerHtml.Trim() : string.Empty;

                var url = string.Format("http://reflex-central.com/tracks/{0}", trackName.Trim());

                if (FileExistsOnServer(url + ".zip"))
                {
                    url += ".zip";
                }
                else if (FileExistsOnServer(url + ".rar"))
                {
                    url += ".rar";
                }

                Track track = new Track
                {
                    TrackName = trackName,
                    TrackUrl = url,
                    ThumbnailUrl = imageUrl,
                    Author = author,
                };

                tracks.Add(track);

            }
            return tracks.ToArray();
        }
    }
}
