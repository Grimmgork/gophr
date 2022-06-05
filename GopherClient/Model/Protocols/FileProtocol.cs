using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Threading;

namespace GopherClient.Model.Protocols
{
	public class FileProtocol : ResourceRequest
	{
		internal override void MakeRequest()
		{
			base.MakeRequest();
			byte[] data = File.ReadAllBytes(Url.LocalPath);
			string type = FileExtension.GetType(Path.GetExtension(Url.LocalPath));
		}
	}
}
