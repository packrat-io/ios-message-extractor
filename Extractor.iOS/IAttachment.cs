namespace Extractor.iOS
{
    public interface IAttachment
    {
        string OriginalFileName { get; set; }

        bool FileExists { get; set; }

        string PathOnDisk { get; set; }

        long Length { get; set; }

        string Checksum { get; set; }
    }
}
