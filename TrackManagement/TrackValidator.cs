﻿using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace TrackManagement
{
    public class TrackValidator
    {
        public Track ValidateTrack(Track track)
        {
            string ext = Path.GetExtension(track.TrackUrl);
            //We can only run automation on zip files. .rar is a closed format and not accepted.
            if (ext == ".zip")
            {
                track = PeekZipFile(track.TrackUrl, track);
            }
            else
            {
                track.ErrorInfo += string.Format("Expected .zip file, got {0} file; ", ext);
                track.Result = ProcessResult.InvalidFileType;
            }

            return track;
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

        private Track PeekZipFile(string url, Track track)
        {
            using (WebClient client = new WebClient())
            {
                using (Stream memoryStream = new MemoryStream(client.DownloadData(url)))
                {
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
                    {
                        track = ValidateZipArchive(archive, track);
                    }
                }
            }

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
                if (entry.FullName.EndsWith(databaseExt, StringComparison.OrdinalIgnoreCase))
                {
                    if (track.TrackType == TrackType.Unknown)
                    {
                        track.TrackType = GetTrackType(entry.FullName);
                        track.SlotNumber = GetSlot(entry.FullName);
                    }
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
                track.Result = ProcessResult.MissingTrackType;
            }

            if (track.SlotNumber == 0)
            {
                track.ErrorInfo += "Unknown slot; ";
                track.Result = ProcessResult.MissingSlot;
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
                track.Result = ProcessResult.InvalidTrackCount;
            }
            else if (timesFileAppearsInZip > 1)
            {
                track.ErrorInfo += string.Format("Expecting 1 *{0} file but got {1}. Please only upload one track per zip file; ", fileExtention, timesFileAppearsInZip);
                track.Result = ProcessResult.InvalidTrackCount;
            }
            return track;
        }
    }
}