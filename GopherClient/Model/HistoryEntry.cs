using System;
using System.Collections.Generic;
using System.Text;

namespace GopherClient.Model
{
	public struct HistoryEntry
	{
		public string url { get; set; }
		public GopherResourceType type { get; set; }

		public static bool operator ==(HistoryEntry a, HistoryEntry b)
		{
			return a.url == b.url && a.type == b.type;
		}

		public static bool operator !=(HistoryEntry a, HistoryEntry b)
		{
			return a.url != b.url || a.type != b.type;
		}

		public override string ToString()
		{
			return $"Entry: {url}, {type}";
		}

	}
}
