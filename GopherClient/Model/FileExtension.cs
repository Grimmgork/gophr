using System;
using System.Collections.Generic;
using System.Text;

namespace GopherClient.Model
{
	public static class FileExtension
	{
		static Dictionary<string, string> map = new Dictionary<string, string>();
		public static void Add(string extension, string type){
			if (map.ContainsKey(extension))
				return;
			map.Add(extension, type);
		}

		public static void Clear(){
			map.Clear();
		}

		public static string GetType(string extension){
			if(map.ContainsKey(extension))
				return map[extension];
			return null;
		}
	}
}