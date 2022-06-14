using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GopherClient.Model
{
    public interface IDataChunkConsumer : IDisposable
    {
        public Task Consume(ResourceRequest request, CancellationToken t);
    }
}
