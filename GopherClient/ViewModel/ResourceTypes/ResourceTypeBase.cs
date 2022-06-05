using System;
using System.Collections.Generic;
using System.Text;

namespace GopherClient.ViewModel.ResourceTypes
{
	public abstract class ResourceTypeBase : OnPropertyChangedBase
	{
		public List<byte[]> Chunks = new List<byte[]>();

		public virtual void AppendData(byte[] chunk, bool lastOne)
		{
			Chunks.Add(chunk);
		}
	}
}
