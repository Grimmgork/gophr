using System;
using System.Collections.Generic;
using System.Text;

namespace GopherClient.Model.Protocols
{
	public interface IScheme
	{
		public Resource GetResource(Uri url);
	}
}
