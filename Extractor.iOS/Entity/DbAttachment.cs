namespace Extractor.iOS.Entity
{
    internal sealed class DbAttachment
    {
        public int attachment_id { get; set; }
        
        public string filename { get; set; }

        public int message_id { get; set; }
    }
}
