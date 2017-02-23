using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Extractor.iOS.Entity;
using SQLite;

namespace Extractor.iOS.Query
{
    internal sealed class GetAttachmentsQuery : IQuery<List<Attachment>>
    {
        private readonly SQLiteAsyncConnection connection;
        private readonly string localPathRoot;

        public GetAttachmentsQuery(SQLiteAsyncConnection connection, string localPathRoot)
        {
            this.connection = connection;
            this.localPathRoot = localPathRoot;
        }

        public async Task<List<Attachment>> Execute(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rawAttachments = await connection.QueryAsync<DbAttachment>(@"
                SELECT maj.attachment_id, maj.message_id, a.filename
                FROM attachment a
                JOIN message_attachment_join maj ON maj.attachment_id = a.rowid");

            var transformedAttachments = new List<Attachment>(rawAttachments.Count);
            rawAttachments.ForEach(a => transformedAttachments.Add(Transform(a)));

            return transformedAttachments;
        }

        private Attachment Transform(DbAttachment stored)
        {
            // Normalize internal naming scheme
            var backupName = string.Empty;
            if (stored.filename.StartsWith("/var/mobile/Library"))
                backupName = stored.filename.Replace("/var/mobile/Library", "MediaDomain-Library");
            else if (stored.filename.StartsWith("~/Library"))
                backupName = stored.filename.Replace("~/Library", "MediaDomain-Library");

            backupName = Sha1Helper.Hash(backupName);

            var fileName = stored.filename.Substring(stored.filename.LastIndexOf('/') + 1);
            var localPath = Path.Combine(localPathRoot, backupName);

            return new Attachment()
            {
                MessageID = stored.message_id,
                OriginalFileName = fileName,
                PathOnDisk = localPath
            };
        }
    }
}
