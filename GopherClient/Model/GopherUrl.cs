using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.IO;

namespace GopherClient.Model
{
    public class GopherUrl
    {
		string _scheme;
		public string Scheme
		{
			get
			{
				return _scheme;
			}
            set
            {
				_scheme = value;
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
				return $"{Path}{Query}";
			}
		}

		public string Path
		{
			get
			{
				if (Segments.Length == 0)
					return "";
				return "/" + String.Join('/', Segments);
			}
		}

		string _query = "";
		public string Query
		{
			get
			{
				if (_query == "")
					return "";
				return "\t" + _query;
			}
			set
			{
				_query = value;
			}
		}

		private string _extension = "";
		public string FileExtension
        {
            get
            {
				return _extension;
            }
			private set
            {
				_extension = value;
            }
        }

		public string[] Segments = new string[] { };

		public GopherUrl()
		{

		}

		public GopherUrl(string i)
		{
			if (i == null || i == String.Empty)
				return;

			Uri uri = new Uri(i);
			Port = uri.Port;
			Scheme = uri.Scheme;

			Host = uri.Host;
			
			FileExtension = System.IO.Path.GetExtension(uri.AbsolutePath);

			Segments = GetSegments(uri.AbsolutePath);

			if (Segments.Length == 0)
			{
				if(Scheme == "gopher")
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

			if (Scheme == "gopher" && (Query == ""|| Query==null))
				Query = ExtractGopherQuery(Segments);
		}

		private static string[] GetSegments(string path)
		{
			return path.Split("/").Where(s => s != "").ToArray();
		}

		private static string ExtractGopherQuery(string[] segments)
		{
			if (segments.Length == 0)
				return "";

			string query = "";
			string lastSegment = segments.Last();

			lastSegment = lastSegment.Replace("%09", "\t");
			int i = lastSegment.IndexOf("\t");
			if (i != -1)
			{
				query = lastSegment.Substring(i+1);
				segments[segments.Length - 1] = lastSegment.Substring(0, i);
			}

			return query;
		}

		public string ToString(bool withType = true)
		{
			if(withType)
				return $"{Scheme}://{Host}:{Port}/{Type}{PathAndQuery}";
			return $"{Scheme}://{Host}:{Port}{PathAndQuery}";
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
