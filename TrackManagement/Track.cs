namespace TrackManagement
{
    public enum TrackType
    {
        National,
        Supercross,
        FreeRide,
        Unknown
    }

    public enum ProcessResult
    {
        InvalidFileType,
        InvalidTrackCount,
        MissingSlot,
        MissingTrackType,
        Success,
    }

    public class Track
    {
        public Track()
        {
            TrackType = TrackType.Unknown;
            TrackName = string.Empty;
            TrackUrl = string.Empty;
            ThumbnailUrl = string.Empty;
            Author = string.Empty;
            SlotNumber = 0;
            ErrorInfo = string.Empty;
            Result = ProcessResult.Success;
        }
        public TrackType TrackType { get; set; }
        public string TrackName { get; set; }
        public string TrackUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Author { get; set; }
        public int SlotNumber { get; set; }

        //Debug info
        public ProcessResult Result { get; set; }
        public string ErrorInfo { get; set; }
    }
}
