namespace Extractor.iOS.Entity
{
    internal sealed class Attachment
    {
        public int MessageID { get; set; }

        public string FileName { get; set; }

        public bool FileExists { get; set; }

        public string PathOnDisk { get; set; }

        public long Length { get; set; }

        public string Checksum { get; set; }
    }
}
