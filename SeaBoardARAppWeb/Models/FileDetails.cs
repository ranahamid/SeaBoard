namespace SB.AR.AppWeb.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// File Details
    /// </summary>
    [Serializable]
    public class FileDetails
    {
        public string FileId { get; set; }
        public string BaseName { get; set; }

        public string FileName { get; set; }

        public byte[] FileContent { get; set; }

        public string FileURL { get; set; }

        public FileStatus Status { get; set; }
    }

    /// <summary>
    /// File Status
    /// </summary>
    public enum FileStatus
    {
        /// <summary>
        /// The no action
        /// </summary>
        NoAction = 0,

        /// <summary>
        /// The new
        /// </summary>
        New = 1,

        /// <summary>
        /// The delete
        /// </summary>
        Delete = 2,
    }
}