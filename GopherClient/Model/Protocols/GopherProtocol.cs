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
		private GopherUrl Url;

		public GopherProtocol(string url, bool waitForDots) : base(url)
        {
			Url = new GopherUrl(url);
			this.waitForDots = waitForDots;
        }

		internal override Task StartRequest(CancellationToken t)
		{
			return Task.Run(() =>
			{
				waitForDots = false;

				const int buffersize = 1024;

				TcpClient tcp = new TcpClient(Url.Host, Url.Port);
				tcp.ReceiveBufferSize = buffersize;
				tcp.ReceiveTimeout = 5000;
				NetworkStream stream = tcp.GetStream();
				stream.Write(Encoding.ASCII.GetBytes(Url.PathAndQuery + "\n"));

				int timeout = 0;
				while(!t.IsCancellationRequested)
                {
					byte[] chunk = new byte[buffersize];
					int lengthOfChunk = stream.Read(chunk, 0, buffersize);

					if (lengthOfChunk == 0)
                    {
						timeout++;
						if (timeout > 10)
							break;
						Thread.Sleep(10);
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