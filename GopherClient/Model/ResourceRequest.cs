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

		internal CancellationToken cancelToken;

		private bool hasType = false;
		private ResourceType type = ResourceType.Unknown;
		private Queue<byte[]> Chunks;

		public static ResourceRequest Request(Uri url, CancellationToken token, ResourceType forceType = ResourceType.Unknown) 
		{
			
			string scheme = url.Scheme;
			ResourceRequest request = null;

			switch (scheme) {
				case "gopher":
					request = new GopherProtocol();
					break;
				case "file":
					request = new FileProtocol();
					break;
				default:
					throw new Exception("Protocol not supported!");
			}

			if (forceType != ResourceType.Unknown)
				request.ReportType(forceType);

			request.cancelToken = token;
			request.Url = url;
			Task.Run(() =>{
				request.MakeRequest();
				request.IsFinished = true;
			}, token);
			return request;
		}

		public async Task<byte[]> AwaitChunk(CancellationToken t)
		{
			return await Task.Run(() => {
				while(true){
					Thread.Sleep(10);
					if (t.IsCancellationRequested) throw new OperationCanceledException();
					if (Chunks.Count != 0 ) break;
					if (IsFinished) return null;
				}
				return GetChunk();
			}, t);
		}

		public async Task<ResourceType> AwaitType(CancellationToken t)
		{
			return await Task.Run(() => {
				while (!hasType) { if (t.IsCancellationRequested || cancelToken.IsCancellationRequested) { throw new OperationCanceledException(); } };
				return type;
			});
		}

		internal ResourceRequest()
		{
			Chunks = new Queue<byte[]>();
		}

		internal virtual void MakeRequest() { }

		internal void ReportType(ResourceType type)
		{
			if (hasType)
				return;

			this.type = type;
			hasType = true;
		}

		internal void ReportChunk(byte[] chunk)
		{
			Size += chunk.Length;
			lock (Chunks){
				Chunks.Enqueue(chunk);
			}
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
