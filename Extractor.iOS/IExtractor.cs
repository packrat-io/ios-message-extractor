using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Extractor.iOS
{
    public interface IExtractor : IDisposable
    {
        Task<IList<IMessage>> ExtractAsync(CancellationToken cancellationToken = default(CancellationToken), IProgress<ExtractProgress> progress = null);
    }
}
