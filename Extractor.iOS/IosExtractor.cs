using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Extractor.iOS.Query;
using Extractor.iOS.Entity;
using System.IO;
using System.Security.Cryptography;

namespace Extractor.iOS
{
    public sealed class IosExtractor : IExtractor
    {
        private const string DefaultMyHandle = "<Me>";

        private const string Stage1Name = "Loading messages";
        private const string Stage2Name = "Loading group messages";
        private const string Stage3Name = "Loading and hashing attachments";
        private const string Stage4Name = "Adding attachment metadata";
        private const int TotalStages = 4; //todo

        private readonly string backupFilePath;
        private readonly string myHandle;

        private bool isDisposed = false;

        public IosExtractor(string backupFilePath, string myHandle = null)
        {
            if (string.IsNullOrEmpty(backupFilePath))
                throw new ArgumentNullException(nameof(backupFilePath));

            this.backupFilePath = backupFilePath;
            this.myHandle = myHandle;

            if (string.IsNullOrEmpty(this.myHandle))
                this.myHandle = DefaultMyHandle;
        }

        public async Task<IReadOnlyList<IMessage>> ExtractAsync(CancellationToken cancellationToken = default(CancellationToken), IProgress<ExtractProgressInfo> progress = null)
        {
            var conn = new SQLite.SQLiteAsyncConnection(this.backupFilePath);

            // Get all messages
            ReportProgress(progress, 1, 0, Stage1Name);
            var messages = await new GetMessagesQuery(conn, myHandle).Execute(cancellationToken);
            ReportProgress(progress, 1, 100, Stage1Name);

            // Get list of messages that are part of group chats
            // and use it to add group chat metadata to existing messages
            ReportProgress(progress, 2, 0, Stage2Name);
            var groupChatMessages = await new GetGroupConversationsQuery(conn).Execute(cancellationToken);
            this.AddGroupMetadata(messages, groupChatMessages, progress);
            ReportProgress(progress, 2, 100, Stage2Name);

            // Get attachment metadata and create checksum hashes
            ReportProgress(progress, 3, 0, Stage3Name);
            var attachments = await new GetAttachmentsQuery(conn, Path.GetDirectoryName(backupFilePath)).Execute(cancellationToken);
            this.HashAttachments(attachments, progress);
            ReportProgress(progress, 3, 100, Stage3Name);

            // Add attachment metadata to existing messages
            ReportProgress(progress, 4, 0, Stage4Name);
            this.AddAttachmentMetadata(messages, attachments, progress);
            ReportProgress(progress, 4, 100, Stage4Name);

            // Done!
            return messages;
        }

        private void AddGroupMetadata(IList<IosMessage> existingMessages, IList<DbGroupChatMessage> groupMessageReferences, IProgress<ExtractProgressInfo> progress)
        {
            var total = groupMessageReferences.Count;
            var current = 0;

            foreach (var groupMessage in groupMessageReferences)
            {
                try
                {
                    var existing = existingMessages.ElementAtOrDefault(groupMessage.message_id);
                    if (existing == null)
                        continue;

                    existing.Participants.Add(groupMessage.handle);

                    if (!string.IsNullOrEmpty(groupMessage.group_id))
                    {
                        if (!string.IsNullOrEmpty(existing.GroupId) &&
                            !string.Equals(existing.GroupId, groupMessage.group_id, StringComparison.InvariantCultureIgnoreCase))
                            throw new ApplicationException($"Message {existing.MessageId} already has group ID {existing.GroupId}, can't add new group ID {groupMessage.group_id}");

                        existing.GroupId = groupMessage.group_id;
                    }

                    ReportProgress(progress, 2, Percent(current++, total), Stage1Name);
                }
                catch (Exception)
                {
                    //todo logging
                    throw;
                }
            }

            //todo log result stats
        }

        private void HashAttachments(IList<Attachment> attachments, IProgress<ExtractProgressInfo> progress)
        {
            var total = attachments.Count;
            var current = 0;

            long attachmentBytes = 0;
            foreach (var file in attachments.Where(x => x.FileExists))
            {
                try
                {
                    var fileInfo = new FileInfo(file.PathOnDisk);

                    attachmentBytes += fileInfo.Length;
                    file.Length = fileInfo.Length;

                    using (var stream = new BufferedStream(fileInfo.OpenRead(), (int)Math.Pow(1024, 3)))
                    using (var hasher = MD5.Create())
                    {
                        file.Checksum = BitConverter.ToString(hasher.ComputeHash(stream)).Replace("-", "").ToLower();
                    }

                    ReportProgress(progress, 3, Percent(current++, total), Stage3Name);
                }
                catch (Exception)
                {
                    // todo logging
                }
            }
        }

        private void AddAttachmentMetadata(IList<IosMessage> existingMessages, IList<Attachment> attachments, IProgress<ExtractProgressInfo> progress)
        {
            var total = attachments.Count;
            var added = 0;

            foreach (var item in attachments.Where(x => x.FileExists))
            {
                try
                {
                    var existing = existingMessages.ElementAtOrDefault(item.MessageID);
                    if (existing == null)
                        continue;

                    existing.Attachments.Add(item);
                    added++;

                    ReportProgress(progress, 4, Percent(added++, total), Stage4Name);
                }
                catch (Exception)
                {
                    // todo logging
                    throw;
                }
            }

            //todo log orphans/missing count
        }

        private static void ReportProgress(
            IProgress<ExtractProgressInfo> progress,
            int currentStage,
            double currentStagePercent,
            string currentStageName)
        {
            if (progress != null)
            {
                var status = new ExtractProgressInfo(currentStage, currentStagePercent, currentStageName, TotalStages);
                progress.Report(status);
            }
        }

        private static double Percent(int current, int total)
            => (current / (double)total) * 100;

        void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // TODO
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
    }
}
