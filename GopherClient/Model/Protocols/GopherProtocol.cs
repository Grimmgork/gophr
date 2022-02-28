using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace GopherClient.Model.Protocols
{
	public class GopherProtocol : IScheme
	{	
		public Resource GetResource(Uri url, CancellationToken token)
		{
			TcpClient tcp = new TcpClient(url.Host, url.Port);
			NetworkStream stream = tcp.GetStream();

			stream.Write(Encoding.ASCII.GetBytes(url.PathAndQuery + "\n"));
			const int buffersize = 6400000;
			byte[] data = new byte[buffersize];
			int index = 0;

			while(true){
				if(token.IsCancellationRequested){
					throw new OperationCanceledException();
				}
				data[index] = (byte) stream.ReadByte();
				index++;
				if (index > 5)
				if(Encoding.ASCII.GetString(data, index-5, 5) == "\r\n.\r\n"){
					break;
				}
			}

			if (token.IsCancellationRequested){
				throw new OperationCanceledException();
			}

			int finallength = index + 1 - 5;
			byte[] finalData = new byte[finallength];
			Array.Copy(data, finalData, finallength);
			string type = null;

			if (url.AbsolutePath == "/" || url.AbsolutePath == ""){
				type = "gopher";
			}

			if (!url.Segments[url.Segments.Length-1].Contains(".")){
				type = "gopher";
			}
			
			return new Resource(finalData, type, url);
		}
	}
}