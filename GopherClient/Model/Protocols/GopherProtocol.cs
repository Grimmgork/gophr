using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace GopherClient.Model.Protocols
{
	public class GopherProtocol : ResourceRequest //IScheme
	{
		internal GopherProtocol() { }

		internal override void MakeRequest()
		{
			base.MakeRequest();
			int timeout = 0;

			if (Url.AbsolutePath == "/" || Url.AbsolutePath == "")
				ReportType(ResourceType.Gopher);
			else
				ReportType(ResourceType.Unknown);

			TcpClient tcp = new TcpClient(Url.Host, Url.Port);
			tcp.ReceiveTimeout = 3000;
			tcp.ReceiveBufferSize = 255;
			NetworkStream stream = tcp.GetStream();
			
			stream.Write(Encoding.ASCII.GetBytes(Url.AbsolutePath + "\n"));

			const int buffersize = 2048;

			int totalIndex = 0;
			int totalChunks = 0;

			byte[] lastChunk = null;

			while (!cancelToken.IsCancellationRequested && tcp.Connected)
			{
				if(!stream.DataAvailable)
				{
					timeout++;
					if (timeout > 100)
						break;

					Thread.Sleep(10);
					continue;
				}

				timeout = 0;
				
				byte[] chunk = new byte[buffersize];
				int lengthOfChunk = stream.Read(chunk, 0, buffersize);
				totalIndex += lengthOfChunk;
				chunk = chunk.Take(lengthOfChunk).ToArray();
				ReportChunk(chunk);
				totalChunks++;

				string lastString = "";
				if(lastChunk == null && chunk.Length >= 5){
					lastString = Encoding.ASCII.GetString(chunk.TakeLast(5).ToArray());
				}
				else{
					if (chunk.Length >= 5){
						lastString = Encoding.ASCII.GetString(chunk.TakeLast(5).ToArray());
					}
					else{
						int l = Math.Min(5, chunk.Length-1); //gopher://sdf.org:70/users/mmww/files/audio/Endgame_volup.ogg
						int c = chunk.Length - l;
						lastString = Encoding.ASCII.GetString(lastChunk.TakeLast(l).ToArray()) + Encoding.ASCII.GetString(chunk.TakeLast(c).ToArray());
					}
				}

				if (lastString == "\r\n.\r\n"){
					break;
				}

				lastChunk = chunk;
				Thread.Sleep(5);
			}

			stream.Close();
			tcp.Close();
		}
	}
}