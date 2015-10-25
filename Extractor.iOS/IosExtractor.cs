using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Extractor.iOS
{
    public sealed class IosExtractor : IExtractor
    {
        private const string DefaultMyHandle = "<Me>";

        private readonly string backupPath;
        private readonly string myHandle;

        private bool isDisposed = false;

        public IosExtractor(string backupPath, string myHandle = null)
        {
            if (string.IsNullOrEmpty(backupPath))
                throw new ArgumentNullException(nameof(backupPath));

            this.backupPath = backupPath;
            this.myHandle = myHandle;

            if (string.IsNullOrEmpty(this.myHandle))
                this.myHandle = DefaultMyHandle;
        }

        public Task<IList<IMessage>> ExtractAsync(CancellationToken cancellationToken = default(CancellationToken), IProgress<ExtractProgress> progress = null)
        {
            throw new NotImplementedException();
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
