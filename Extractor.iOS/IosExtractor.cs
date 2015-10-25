using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Extractor.iOS.Query;

namespace Extractor.iOS
{
    public sealed class IosExtractor : IExtractor
    {
        private const string DefaultMyHandle = "<Me>";

        private const string Stage1Name = "Loading messages";
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

        public async Task<IList<IMessage>> ExtractAsync(CancellationToken cancellationToken = default(CancellationToken), IProgress<ExtractProgress> progress = null)
        {
            var conn = new SQLite.SQLiteAsyncConnection(this.backupFilePath);

            ReportProgress(progress, 1, 0, Stage1Name, TotalStages);
            var messages = await new GetMessagesQuery(conn, myHandle).Execute();
            ReportProgress(progress, 1, 100, Stage1Name, TotalStages);


        }

        private static void ReportProgress(
            IProgress<ExtractProgress> progress,
            int currentStage,
            int currentStagePercent,
            string currentStageName,
            int totalStages)
        {
            if (progress != null)
            {
                var status = new ExtractProgress(currentStage, currentStagePercent, currentStageName, totalStages);
                progress.Report(status);
            }
        }

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
