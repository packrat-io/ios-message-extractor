namespace Extractor.iOS.Entity
{
    internal sealed class DbGroupChatMessage
    {
        public int message_id { get; set; }

        public string service_name { get; set; }

        public string handle { get; set; }

        public string group_id { get; set; }
    }
}
