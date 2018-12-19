namespace TrackManagement
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
            TrackType = TrackType.Unknown;
            TrackName = string.Empty;
            DarkSlidesTrackUrl = string.Empty;
            SlotNumber = 0;
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
