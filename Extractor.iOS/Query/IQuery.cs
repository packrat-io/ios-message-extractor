using System.Threading;
using System.Threading.Tasks;

namespace Extractor.iOS.Query
{
    internal interface IQuery<T>
    {
        Task<T> Execute(CancellationToken cancellationToken);
    }
}
