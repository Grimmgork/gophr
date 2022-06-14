using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GopherClient.Model;

namespace GopherClient.ViewModel.BrowserPages
{
    public class BrowserPageBase : OnPropertyChangedBase, IDataChunkConsumer
    {
        int _dataSize = 0;
        public int DataSize
        {
            get
            {
                return _dataSize;
            }
            internal set
            {
                if(_dataSize != value)
                {
                    _dataSize = value;
                    OnPropertyChanged("DataSize");
                }
            }
        }

        public virtual Task Consume(ResourceRequest request, CancellationToken t)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
