using System;
using System.Collections.Generic;
using System.Text;

namespace GopherClient.Model
{
	public interface IScheme
	{
		public byte[] GetResource(Uri url);
	}
}
