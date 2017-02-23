namespace Extractor.iOS.Entity
{
    internal sealed class Attachment : IAttachment
    {
        public int MessageID { get; set; }

        public string OriginalFileName { get; set; }

        public bool FileExists { get; set; }

        public string PathOnDisk { get; set; }

        public long Length { get; set; }

        public string Checksum { get; set; }
    }
}
