using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GopherClient.Model.Protocols
{
	public interface IScheme
	{
		public Resource GetResource(Uri url, CancellationToken token);
	}
}
