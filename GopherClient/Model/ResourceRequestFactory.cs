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
		public static ResourceRequest NewRequest(Uri url, GopherResourceType forceType = GopherResourceType.Unknown)
		{
			string scheme = url.Scheme;
			ResourceRequest request = null;
			switch (scheme)
			{
				case "gopher":
					bool waitForDots = (forceType == GopherResourceType.Gopher || forceType == GopherResourceType.Text);
					waitForDots = true;
					request = new GopherProtocol(url, waitForDots);
					break;
				default:
					throw new Exception("Protocol not supported!");
			}

			return request;
		}
	}
}
