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

		public GopherProtocol(Uri uri, bool waitForDots) : base(uri)
        {
			this.waitForDots = waitForDots;
        }

		public static Tuple<string, char> ExtractTypeFromUrl(string url)
        {
			Uri uri = new Uri(url);
			char type = '.';
			string[] segments = uri.AbsolutePath.Split("/");
			segments = segments.Where(s => s != "").ToArray();

			if(segments.Length > 0)
            {
				if (segments[0].Length == 1)
				{
					type = segments[0][0];
					segments = segments.Skip(1).ToArray();
				}
			}

			if(segments.Length == 0 && type == '.') {
				type = '1';
			}

			url = $"{uri.Scheme}://{uri.Host}/{String.Join("/", segments)}";
			return new Tuple<string, char>(url, type);
		}

		public static string GenrateUrl(string host, string path, char type, int port = 70)
        {
			path = String.Join('/', path.Split("/").Where(s => s != ""));
			return EmbedTypeInUrl($"gopher://{host}/{path}", type);
		}

		public static string EmbedTypeInUrl(string url, char type)
        {
			Uri uri = new Uri(url);
			string[] segments = uri.AbsolutePath.Split("/");
			segments = segments.Where(s => s != "").ToArray();
			if (segments.Length > 0 && segments[0].Length == 1)
				segments[0] = type.ToString();
			else
				segments = segments.Prepend(type.ToString()).ToArray();

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

			return EmbedTypeInUrl($"{uri.Scheme}://{uri.Host}/{String.Join("/", segments)}", '1');
		}

		public static string GetServerMainPage(string url)
        {
			return EmbedTypeInUrl(new Uri(ExtractTypeFromUrl(url).Item1).GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped), '1'); 
        }


		internal override Task StartRequest(CancellationToken t)
		{
			return Task.Run(() =>
			{
				waitForDots = false;

				const int buffersize = 1024;
				string path = Url.AbsolutePath;

				TcpClient tcp = new TcpClient(Url.Host, Url.Port);
				tcp.ReceiveBufferSize = buffersize;
				tcp.ReceiveTimeout = 1000;
				NetworkStream stream = tcp.GetStream();
				stream.Write(Encoding.ASCII.GetBytes(path + "\n"));

				while(!t.IsCancellationRequested)
                {
					byte[] chunk = new byte[buffersize];
					int lengthOfChunk = stream.Read(chunk, 0, buffersize);

					if (lengthOfChunk == 0)
						break;

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