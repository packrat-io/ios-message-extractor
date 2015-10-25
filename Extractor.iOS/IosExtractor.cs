using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Extractor.iOS
{
    public sealed class IosExtractor : IExtractor
    {
        private const string DefaultMyHandle = "<Me>";

        private readonly string backupBasePath;
        private readonly string myHandle;

        public IosExtractor(string backupBasePath, string myHandle = null)
        {
            if (string.IsNullOrEmpty(backupBasePath))
                throw new ArgumentNullException(nameof(backupBasePath));

            this.backupBasePath = backupBasePath;
            this.myHandle = myHandle;

            if (string.IsNullOrEmpty(this.myHandle))
                this.myHandle = DefaultMyHandle;
        }

        public Task<IList<IMessage>> ExtractAsync(CancellationToken cancellationToken = default(CancellationToken), IProgress<ExtractProgress> progress = null)
        {
            throw new NotImplementedException();
        }
    }
}
