using System;
using System.Collections.Generic;
using System.Text;

namespace GopherClient.Model
{
	public static class ResourceRequester
	{
		private static Dictionary<string, IScheme> map = new Dictionary<string, IScheme>();
		
		public static void Map(string protocolname, IScheme scheme){
			map.Add(protocolname, scheme);
		}

		public static void ClearMap(){
			map.Clear();
		}

		public static byte[] GetResource(Uri url)
		{
			return map[url.Scheme].GetResource(url);
		}
	}
}
