using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace GopherClient.Model
{
	public class FileProtocol : IScheme
	{
		public byte[] GetResource(Uri url)
		{
			return File.ReadAllBytes(url.LocalPath);
		}
	}
}
