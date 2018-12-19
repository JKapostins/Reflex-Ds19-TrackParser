using System;
using System.Collections.Generic;
using System.Text;

namespace DarkSlidesTrackListParser
{
    public enum TrackType
    {
        National,
        Supercross,
        FreeRide,
        Unknown
    }

    public class Track
    {
        public Track()
        {
            ErrorInfo = string.Empty;
            Valid = true;
        }
        public TrackType TrackType { get; set; }
        public string TrackName { get; set; }
        public string DarkSlidesTrackUrl { get; set; }
        public int SlotNumber { get; set; }

        //Debug info
        public bool Valid { get; set; }
        public string ErrorInfo { get; set; }
    }
}
