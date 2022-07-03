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
		public static readonly string configFilePath = "config.json";
		public string startUrl { get; internal set; }
		public string[] bookmarks { get; internal set; }

		public string[] trustetProtocols { get; internal set; }
		public string[] trustedFileExtensions { get; internal set; }

		public Dictionary<string, string> typeMappings { get; set; }


		public static Configuration Default(){
			return new Configuration()
			{
				startUrl = "gopher://gopher.floodgap.com",
				trustetProtocols = new string[] { "https", "http", "gopher" },
				trustedFileExtensions = new string[] { ".txt", ".jpg", ".png", ".gif", ".html" },
				typeMappings = new Dictionary<string, string>() { 
					{ "text/plain", @"c:\users\eric\projects\gophr\GopherClient\apps\text-plain.cmd" },
				}
			};
		}
		
		public static Configuration Load(){

			return Default();

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