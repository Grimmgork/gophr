using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GopherClient.Model.Protocols
{
	public interface IScheme
	{
		public void GetResource(Uri url, CancellationToken token, Action<byte[]> addChunkAction, Action finishedCallback);
	}
}
