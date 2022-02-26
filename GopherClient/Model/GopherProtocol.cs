using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace GopherClient.Model
{
	public class GopherProtocol : IScheme
	{	
		public byte[] GetResource(Uri url)
		{
			TcpClient tcp = new TcpClient(url.Host, url.Port);
			NetworkStream stream = tcp.GetStream();

			stream.Write(Encoding.ASCII.GetBytes(url.PathAndQuery + "\n"));
			const int buffersize = 10000;
			byte[] data = new byte[buffersize];
			int index = 0;

			while(true){
				data[index] = (byte) stream.ReadByte();
				index++;
				if (index > 5)
				if(Encoding.ASCII.GetString(data, index-5, 5) == "\r\n.\r\n"){
					break;
				}
			}

			int finallength = index + 1 - 5;
			byte[] result = new byte[finallength];
			Array.Copy(data, result, finallength);

			return result;
		}
	}
}