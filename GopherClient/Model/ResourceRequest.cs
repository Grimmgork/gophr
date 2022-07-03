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

		GopherUrl _uri;
		public GopherUrl Url
        {
            get
            {
				return _uri;
            }
			internal set
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

		private string mimeType = "application/unknown";
		private bool mimeTypeReported = false;
		
		private Queue<byte[]> Chunks = new Queue<byte[]>();

		public ResourceRequest(GopherUrl url)
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
					//Thread.Sleep(5);
				}
				return null;
			}, t);
		}

		public Task<string> AwaitMimeType(CancellationToken t)
		{
			return Task.Run(() => {
				while (!IsFinished)
				{ 
					if (t.IsCancellationRequested || mimeTypeReported || IsFinished)
						break;
					Thread.Sleep(5);
				}
				return mimeType;
			});
		}

		internal virtual Task StartRequest(CancellationToken t) 
		{
			throw new NotImplementedException();
		}

		internal void ReportMimeType(string type)
		{
			mimeTypeReported = true;
			if (type == "")
				return;

            lock (type)
            {
				this.mimeType = type;
			}
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
			mimeTypeReported = true;
        }

		private byte[] GetChunk()
		{
			if (Chunks.Count == 0)
				return null;

			lock (Chunks)
			{
				return Chunks.Dequeue();
			}
		}
	}
}
