using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GopherClient.Model
{
    public class GopherUrl
    {
		public string Scheme
		{
			get
			{
				return "gopher";
			}
		}

		string _host = "";
		public string Host
		{
			get
			{
				return _host;
			}
			set
			{
				_host = value;
			}
		}

		char _type = '.';
		public char Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		int _port = 70;
		public int Port
		{
			get
			{
				return _port;
			}
			set
			{
				_port = value;
			}
		}

		public string PathAndQuery
		{
			get
			{
				return $"{Path}{ (Query == String.Empty ? "" : $"?{Query}") }";
			}
		}

		public string Path
		{
			get
			{
				return String.Join('/', Segments);
			}
		}

		string _query = "";
		public string Query
		{
			get
			{
				return _query;
			}
			set
			{
				_query = value;
			}
		}

		public string[] Segments = new string[] { };

		public GopherUrl()
		{

		}

		public GopherUrl(string i)
		{
			Uri uri = new Uri(i);
			Port = uri.Port;
			if (uri.Scheme != "gopher")
				throw new Exception("Not a gopher url!");

			Host = uri.Host;

			Segments = GetSegments(uri.AbsolutePath);

			if (Segments.Length == 0)
			{
				Type = '1';
				return;
			}

			if (Segments[0].Length == 1)
			{
				Type = Segments[0][0];
				Segments = Segments.Skip(1).ToArray();
			}

			if (Segments.Length == 0)
				return;

			if (uri.Query == string.Empty || uri.Query == null)
			{
				Query = ExtractQuery(Segments);
			}
		}

		private static string[] GetSegments(string path)
		{
			return path.Split("/").Where(s => s != "").ToArray();
		}

		private static string ExtractQuery(string[] segments)
		{
			string result = "";
			if (segments.Last().Contains("%09"))
			{
				string[] s = segments.Last().Split("%09");
				result = s[1];
				segments[segments.Length - 1] = s[0];
			}
			else
				if (segments.Last().Contains("%3F"))
			{
				string[] s = segments.Last().Split("%3F");
				result = s[1];
				segments[segments.Length - 1] = s[0];
			}

			return result;
		}

		public override string ToString()
		{
			return $"gopher://{Host}:{Port}/{Type}/{PathAndQuery}";
		}

		public string UrlWithoutType()
		{
			return $"gopher://{Host}:{Port}/{PathAndQuery}";
		}

		public GopherUrl GetServerRoot()
        {
			GopherUrl res = new GopherUrl(this.ToString());
			res.Segments = new string[] { };
			res.Type = '1';
			return res;
		}

		public GopherUrl GetOneAbove()
        {
			GopherUrl res = new GopherUrl(this.ToString());
			if(res.Segments.Length > 0)
				res.Segments = res.Segments.Take(res.Segments.Length - 1).ToArray();
			return res;
        }
	}
}
