using System;
using System.Collections.Generic;

namespace Extractor.iOS
{
    public interface IMessage
    {
        string MessageId { get; }

        int NaturalIndex { get; }

        DateTimeOffset Date { get; }

        DateTimeOffset? DateDelivered { get; }

        DateTimeOffset? DateRead { get; }

        string From { get; }

        string To { get; }

        IList<string> Participants { get; }

        string GroupId { get; }

        bool FromMe { get; }

        string ServiceName { get; }

        string Subject { get; }

        string Text { get; }

        IList<IAttachment> Attachments { get; }
    }
}
