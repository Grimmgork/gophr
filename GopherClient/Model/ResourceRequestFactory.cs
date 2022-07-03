using GopherClient.Model.Protocols;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GopherClient.Model
{
    public static class ResourceRequestFactory
    {
		public static ResourceRequest NewRequest(GopherUrl url)
		{
			string scheme = url.Scheme;
			switch (scheme)
			{
				case "gopher":
					return new GopherProtocol(url, false);
				default:
					throw new Exception("Protocol not supported!");
			}
		}
	}
}
