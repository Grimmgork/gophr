using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GopherClient.Model.Protocols;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GopherClient.Model
{
	public abstract class ResourceRequest {

		bool _isFinshed;
		public bool IsFinished
		{
			get
			{
				return _isFinshed;
			}
			private set
			{
				_isFinshed = value;
			}
		}

		Uri _uri;
		public Uri Url
        {
            get
            {
				return _uri;
            }
			private set
            {
				_uri = value;
            }
        }

		int _size;
		public int Size{
			get{
				return _size;
			}
			private set{
				_size = value;
			}
		}

		private bool _hasType = false;
		public bool HasType
        {
            get
            {
				return _hasType;
            }
			private set
            {
				_hasType = value;
            }
        }

		
		private GopherResourceType type = GopherResourceType.Unknown;
		private Queue<byte[]> Chunks = new Queue<byte[]>();

		public ResourceRequest(Uri url)
        {
			this.Url = url;
        }

		public Task<byte[]> AwaitNextChunk(CancellationToken t)
		{
			return Task.Run<byte[]>(() => {
				while(true){
					if (IsFinished && Chunks.Count == 0) return null;
					if (t.IsCancellationRequested) throw new OperationCanceledException();
					if (Chunks.Count != 0 ) return GetChunk();
				}
			}, t);
		}

		public Task<GopherResourceType> AwaitType(CancellationToken t)
		{
			return Task.Run(() => {
				while (!IsFinished)
				{
					if(t.IsCancellationRequested)
						throw new OperationCanceledException();
					if (HasType) 
						return type;
				}
				return type;
			});
		}

		internal virtual Task StartRequest(CancellationToken t) 
		{
			return null;
		}

		internal void ReportType(GopherResourceType type)
		{
			this.type = type;
			HasType = true;
		}

		internal void ReportChunk(byte[] chunk)
		{
			if (chunk == null || chunk.Length == 0)
				return;

			Size += chunk.Length;
			lock (Chunks){
				Chunks.Enqueue(chunk);
			}
		}

		internal void ReportEnd()
        {
			IsFinished = true;
        }

		private byte[] GetChunk()
		{
			if (Chunks.Count == 0)
				return null;

			lock (Chunks)
			{
				return Chunks.Dequeue(); ;
			}
		}
	}
}
