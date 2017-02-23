using System;
using System.Collections.Generic;

namespace Extractor.iOS.Entity
{
    public sealed class IosMessage : IMessage
    {
        public IList<IAttachment> Attachments { get; internal set; } = new List<IAttachment>();

        public DateTimeOffset Date { get; internal set; }

        public DateTimeOffset? DateDelivered { get; internal set; }

        public DateTimeOffset? DateRead { get; internal set; }

        public string From { get; internal set; }

        public bool FromMe { get; internal set; }

        public string GroupId { get; internal set; }

        public int NaturalIndex { get; internal set; }

        public string MessageId { get; internal set; }

        public IList<string> Participants { get; internal set; }

        public string ServiceName { get; internal set; }

        public string Subject { get; internal set; }

        public string Text { get; internal set; }

        public string To { get; internal set; }
    }
}
