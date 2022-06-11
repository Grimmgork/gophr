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
	public class GopherProtocol : ResourceRequest //IScheme
	{
		internal GopherProtocol() { }

		public static Tuple<string, GopherResourceType> ExtractTypeFromUrl(string url)
        {
			Uri uri = new Uri(url);
			GopherResourceType type = GopherResourceType.Unknown;
			string[] segments = uri.AbsolutePath.Split("/");
			segments = segments.Where(s => s != "").ToArray();

			if(segments.Length > 0)
            {
				if (segments[0].Length == 1)
				{
					type = ResourceTypeMap.GetResourceType(segments[0][0]);
					segments = segments.Skip(1).ToArray();
				}
			}

			if(segments.Length == 0 && type == GopherResourceType.Unknown) {
				type = GopherResourceType.Gopher;
			}

			url = $"{uri.Scheme}://{uri.Host}/{String.Join("/", segments)}";
			return new Tuple<string, GopherResourceType>(url, type);
		}

		public static string GenrateUrl(string host, string path, char type, int port = 70)
        {
			path = String.Join('/', path.Split("/").Where(s => s != ""));
			return EmbedTypeInUrl($"gopher://{host}/{path}", ResourceTypeMap.GetResourceType(type));
		}

		public static string EmbedTypeInUrl(string url, GopherResourceType type)
        {
			Uri uri = new Uri(url);
			string[] segments = uri.AbsolutePath.Split("/");
			segments = segments.Where(s => s != "").ToArray();
			string identifier = ResourceTypeMap.GetResourceIdentifier(type).ToString();
			if (segments.Length > 0 && segments[0].Length == 1)
				segments[0] = identifier;
			else
				segments = segments.Prepend(ResourceTypeMap.GetResourceIdentifier(type).ToString()).ToArray();

			string result = $"{uri.Scheme}://{uri.Host}/{String.Join("/", segments)}";
			return result;
        }

		public static string GetOneAbove(string url)
        {
			Uri uri = new Uri(url);
			string[] segments = uri.AbsolutePath.Split("/").Where(s => s != "").ToArray();
			if(segments.Length > 0)
            {
				segments = segments.Take(segments.Length - 1).ToArray();
            }

			return EmbedTypeInUrl($"{uri.Scheme}://{uri.Host}/{String.Join("/", segments)}", GopherResourceType.Gopher);
		}

		public static string GetServerMainPage(string url)
        {
			return EmbedTypeInUrl(new Uri(ExtractTypeFromUrl(url).Item1).GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped), GopherResourceType.Gopher); 
        }
			
		public async IAsyncEnumerable<byte[]> RequestData(int buffersize, CancellationToken t)
        {
			TcpClient tcp = new TcpClient("gopher.floodgap.com", 70);
			NetworkStream stream = tcp.GetStream();
			stream.Write(Encoding.ASCII.GetBytes("/gopher\n"));
			Thread.Sleep(1000);
			while(!t.IsCancellationRequested && stream.DataAvailable)
            {
				byte[] chunk = new byte[buffersize];
				await stream.ReadAsync(chunk, 0, buffersize);
				yield return chunk;
			}
		}

		internal override void MakeRequest()
		{
			base.MakeRequest();

			string path = Url.AbsolutePath;

			TcpClient tcp = new TcpClient(Url.Host, Url.Port);
			tcp.ReceiveTimeout = 3000;
			tcp.ReceiveBufferSize = 255;
			NetworkStream stream = tcp.GetStream();
			stream.Write(Encoding.ASCII.GetBytes(path + "\n"));

			const int buffersize = 255;

			while (!cancelToken.IsCancellationRequested && tcp.Connected)
			{
				
				bool timeout = false;
				DateTime start = DateTime.Now;
				while(!stream.DataAvailable) 
				{
					if (DateTime.Now.Subtract(start).Seconds > 2){
						timeout = true;
						break;
					}
				}

				if (timeout)
					break;
				
				byte[] chunk = new byte[buffersize];
				int lengthOfChunk = stream.Read(chunk, 0, buffersize);
				chunk = chunk.Take(lengthOfChunk).ToArray();
				ReportChunk(chunk);
			}

			stream.Close();
			tcp.Close();
		}
	}
}