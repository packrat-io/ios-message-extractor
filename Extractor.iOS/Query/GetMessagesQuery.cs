using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Extractor.iOS.Entity;
using SQLite;
using System;
using System.Threading;

namespace Extractor.iOS.Query
{
    internal sealed class GetMessagesQuery : IQuery<IList<IosMessage>>
    {
        private static readonly DateTimeOffset AppleEpoch =
            new DateTimeOffset(1970, 01, 01, 00, 00, 00, TimeSpan.Zero).AddSeconds(978307200);

        private readonly SQLiteAsyncConnection connection;
        private readonly string myHandle;

        public GetMessagesQuery(SQLiteAsyncConnection connection, string myHandle)
        {
            this.connection = connection;
            this.myHandle = myHandle;
        }

        public async Task<IList<IosMessage>> Execute(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rawMessages = await this.connection.QueryAsync<DbMessage>(@"
                SELECT
                m.ROWID, m.guid, m.text,
                m.subject, h.id as handle, m.account as my_handle,
                m.date, m.date_read, m.date_delivered,
                m.service, m.is_from_me, m.item_type, m.other_handle
                FROM message m
                LEFT JOIN handle h on h.rowid = m.handle_id
                ORDER BY m.rowid");

            // Remove null duplicate messages
            rawMessages = rawMessages.Where(x => !(
                    string.IsNullOrEmpty(x.text) &&
                    x.item_type == 1 &&
                    x.other_handle > -1
                )).ToList();

            cancellationToken.ThrowIfCancellationRequested();

            var transformedMessages = new List<IosMessage>(rawMessages.Count);
            rawMessages.ForEach(m => transformedMessages[m.ROWID] = this.Transform(m));

            return transformedMessages;
        }

        private IosMessage Transform(DbMessage stored)
        {
            var message = new IosMessage()
            {
                MessageId = stored.guid.ToString(),
                NaturalIndex = stored.ROWID,
                Text = stored.text,
                Subject = stored.subject,
                ServiceName = stored.service,
                FromMe = stored.is_from_me,
            };

            // Sanitize date
            message.Date = AppleEpoch.AddSeconds(stored.date);

            if (stored.date_delivered > 0)
                message.DateDelivered = AppleEpoch.AddSeconds(stored.date_delivered);
            if (stored.date_read > 0)
                message.DateRead = AppleEpoch.AddSeconds(stored.date_read);

            // Sanitize handles and add them to participant list
            var sanitizedHandle = ParseAccountHandle(stored.handle);
            var myHandle = this.myHandle ?? ParseAccountHandle(stored.my_handle);

            if (stored.is_from_me)
                message.To = sanitizedHandle;
            else
                message.From = sanitizedHandle;

            message.Participants = new List<string>();

            if (!string.IsNullOrEmpty(sanitizedHandle))
                message.Participants.Add(sanitizedHandle);

            message.Participants.Add(myHandle);

            return message;
        }

        private static string ParseAccountHandle(string accountHandle)
        {
            if (string.IsNullOrEmpty(accountHandle))
                return null;

            var trimmed = string.Empty;

            if (accountHandle.StartsWith("p:") ||
                accountHandle.StartsWith("e:"))
                trimmed = accountHandle.Substring(2);

            else if (accountHandle.StartsWith("tel:"))
                trimmed = accountHandle.Substring(4);

            else
                trimmed = accountHandle;

            return string.IsNullOrWhiteSpace(trimmed)
                ? null
                : trimmed;
        }
    }
}
