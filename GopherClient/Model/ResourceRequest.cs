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

		string _uri;
		public string Url
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


		private char type = '.';
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

		
		private Queue<byte[]> Chunks = new Queue<byte[]>();

		public ResourceRequest(string url)
        {
			this.Url = url;
        }

		public Task<byte[]> AwaitNextChunk(CancellationToken t)
		{
			return Task.Run<byte[]>(() => {
				while(true){
					if (IsFinished && Chunks.Count == 0) return null;
					if (t.IsCancellationRequested) break;
					if (Chunks.Count > 0) return GetChunk();
					Thread.Sleep(5);
				}
				return null;
			}, t);
		}

		public Task<char> AwaitType(CancellationToken t)
		{
			return Task.Run(() => {
				while (!IsFinished)
				{
					if (HasType)
						return type;
					if (t.IsCancellationRequested)
						break;
					Thread.Sleep(5);
				}
				return type;
			});
		}

		internal virtual Task StartRequest(CancellationToken t) 
		{
			return null;
		}

		internal void ReportType(char type)
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
