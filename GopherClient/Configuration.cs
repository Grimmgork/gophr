using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Reflection;
using System.Diagnostics;

namespace GopherClient
{
	[System.Serializable]
	public class Configuration
	{
		public static string configFilePath = "config.json";
		public string startUrl { get; internal set; }
		public string[] bookmarks { get; internal set; }

		public static Configuration Default(){
			return new Configuration()
			{
				startUrl = "kek!",
			};
		}
		
		public static Configuration Load(){
			string fullpath = Directory.GetParent(Assembly.GetExecutingAssembly().Location) +"\\"+ configFilePath;

			if (File.Exists(fullpath)){
				return (Configuration) JsonSerializer.Deserialize(File.ReadAllText(fullpath), typeof(Configuration));
			}
			return Default();
		}

		public static void Save(Configuration config){
			string fullpath = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "\\" + configFilePath;
			File.WriteAllText(fullpath, JsonSerializer.Serialize(config));
		}
	}
}
