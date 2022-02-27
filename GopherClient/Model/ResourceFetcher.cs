using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GopherClient.Model.Protocols;

namespace GopherClient.Model
{
	public static class ResourceFetcher
	{
	    static Dictionary<string, IScheme> map;
		
		public static void Init(){
			map = new Dictionary<string, IScheme>();
			MapProtocol("gopher", new GopherProtocol());
			MapProtocol("file", new FileProtocol());
		}

		private static void MapProtocol(string protocolname, IScheme scheme){
			map.Add(protocolname, scheme);
		}

		public static Resource Request(string url, CancellationToken token)
		{
			Uri uri = new Uri(url);
			return map[uri.Scheme].GetResource(uri, token);
		}
	}

	public class Resource{

		Uri _uri;
		public Uri Uri{
			get{ return _uri; }
		}
		
		string _type;
		public string Type{
			get{ return _type; }
		}
		byte[] _data;
		public byte[] Data{
			get{ return _data; }
		}

	    public Resource(byte[] data, string type, Uri uri){
			_type = type;
			_data = data;
			_uri = uri;
		}
	}
}
