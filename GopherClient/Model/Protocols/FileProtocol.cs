using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Threading;

namespace GopherClient.Model.Protocols
{
	public class FileProtocol : IScheme
	{
		public Dictionary<string, string> types;

		public Resource GetResource(Uri url, CancellationToken token)
		{
			byte[] data = File.ReadAllBytes(url.LocalPath);
			string[] splitbydots = url.LocalPath.Split('.');
			string extension = splitbydots[splitbydots.Length - 1];
			string type = null;
			if (types.ContainsKey(extension))
				type = types[extension];
			
			return new Resource(data, type, url);
		}
	}
}
