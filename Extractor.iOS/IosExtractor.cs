using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Extractor.iOS.Query;
using Extractor.iOS.Entity;

namespace Extractor.iOS
{
    public sealed class IosExtractor : IExtractor
    {
        private const string DefaultMyHandle = "<Me>";

        private const string Stage1Name = "Loading messages";
        private const string Stage2Name = "Loading group messages";
        private const int TotalStages = 7; //todo

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

        public async Task<IList<IMessage>> ExtractAsync(CancellationToken cancellationToken = default(CancellationToken), IProgress<ExtractProgressInfo> progress = null)
        {
            var conn = new SQLite.SQLiteAsyncConnection(this.backupFilePath);

            // Get all messages
            ReportProgress(progress, 1, 0, Stage1Name, TotalStages);
            var messages = await new GetMessagesQuery(conn, myHandle).Execute(cancellationToken);
            ReportProgress(progress, 1, 100, Stage1Name, TotalStages);

            // Get list of messages that are part of group chats
            // and use it to add group chat metadata to existing messages
            ReportProgress(progress, 2, 0, Stage2Name, TotalStages);
            var groupChatMessages = await new GetGroupConversationsQuery(conn).Execute(cancellationToken);
            this.AddGroupMetadata(messages, groupChatMessages, progress);



        }

        private void AddGroupMetadata(IList<IosMessage> existingMessages, IList<DbGroupChatMessage> groupMessageReferences, IProgress<ExtractProgressInfo> progress)
        {
            var total = groupMessageReferences.Count;
            var current = 0;

            foreach (var groupMessage in groupMessageReferences)
            {
                try
                {
                    var existing = existingMessages[groupMessage.message_id];
                    existing.Participants.Add(groupMessage.handle);

                    if (!string.IsNullOrEmpty(groupMessage.group_id))
                    {
                        if (!string.IsNullOrEmpty(existing.GroupId) &&
                            !string.Equals(existing.GroupId, groupMessage.group_id, StringComparison.InvariantCultureIgnoreCase))
                            throw new ApplicationException($"Message {existing.MessageId} already has group ID {existing.GroupId}, can't add new group ID {groupMessage.group_id}");

                        existing.GroupId = groupMessage.group_id;
                    }

                    ReportProgress(progress, 2, Percent(current++, total), Stage1Name, TotalStages);
                }
                catch (Exception)
                {
                    //todo logging
                    throw;
                }
            }

            //todo log result stats
        }

        private static void ReportProgress(
            IProgress<ExtractProgressInfo> progress,
            int currentStage,
            double currentStagePercent,
            string currentStageName,
            int totalStages)
        {
            if (progress != null)
            {
                var status = new ExtractProgressInfo(currentStage, currentStagePercent, currentStageName, totalStages);
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
                    // TODO: dispose managed state (managed objects).
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
