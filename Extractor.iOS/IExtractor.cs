using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Extractor.iOS
{
    public interface IExtractor : IDisposable
    {
        Task<IReadOnlyList<IMessage>> ExtractAsync(CancellationToken cancellationToken = default(CancellationToken), IProgress<ExtractProgressInfo> progress = null);
    }
}
