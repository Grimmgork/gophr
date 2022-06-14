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
		public static ResourceRequest NewRequest(Uri url, CancellationToken t, char forceType = '.')
		{
			string scheme = url.Scheme;
			ResourceRequest request = null;
			switch (scheme)
			{
				case "gopher":
					bool waitForDots = (forceType == '1' || forceType == '0');
					waitForDots = true;
					request = new GopherProtocol(url, waitForDots);
					request.StartRequest(t);
					break;
				default:
					throw new Exception("Protocol not supported!");
			}

			return request;
		}
	}
}
