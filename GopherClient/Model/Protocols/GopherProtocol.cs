using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace GopherClient.Model.Protocols
{
	public class GopherProtocol : ResourceRequest
	{
		private bool waitForDots = false;

		private Dictionary<char, string> GopherMimeMapping = new Dictionary<char, string>() { 
			{ '0', "text/plain" },
			{ '1', "text/gopher" },
			{ 'g', "image/gif" },
			{ 'I', "image" },
			{ '7', "text/gopher" }
		};

		public GopherProtocol(GopherUrl gurl, bool waitForDots) : base(gurl)
        {
			this.waitForDots = waitForDots;
        }

		internal override Task StartRequest(CancellationToken t)
		{
			return Task.Run(() =>
			{
				waitForDots = false;

				string mimeType = "";
				if(GopherMimeMapping.ContainsKey(Url.Type))
					mimeType = GopherMimeMapping[Url.Type];

				ReportMimeType(mimeType);

				const int buffersize = 2048;

				TcpClient tcp = new TcpClient(Url.Host, Url.Port);
				tcp.ReceiveBufferSize = buffersize;
				tcp.ReceiveTimeout = 5000;
				NetworkStream stream = tcp.GetStream();
				stream.Write(Encoding.ASCII.GetBytes(Uri.UnescapeDataString(Url.PathAndQuery) + "\n"));

				int timeout = 0;
				while(!t.IsCancellationRequested)
                {
					byte[] chunk = new byte[buffersize];
					int lengthOfChunk = stream.Read(chunk, 0, buffersize);

					if (lengthOfChunk == 0)
                    {
						timeout++;
						if (timeout > 100)
							break;
						Thread.Sleep(5);
						continue;
					}
					else
						timeout = 0;

					chunk = chunk.Take(lengthOfChunk).ToArray();
					ReportChunk(chunk);
					Trace.WriteLine($"chunk read! {lengthOfChunk}");
                }

				stream.Close();
				tcp.Close();

				ReportEnd();
				Trace.WriteLine("Done!");
			});
		}
    }
}