using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Documents;

namespace GopherClient.ViewModel.ResourceTypes
{
	public class GopherResource : ResourceTypeBase
	{
		FlowDocument _doc;
		public FlowDocument Doc
		{
			get
			{
				return _doc;
			}
			set
			{
				_doc = value;
				OnPropertyChanged("Doc");
			}
		}

		public GopherResource(byte[] data)
		{
			Doc = GenerateGopherDocument(data);
		}

		public static FlowDocument GenerateGopherDocument(byte[] data)
		{
			string text = Encoding.ASCII.GetString(data);
			string[] lines = text.Split("\n");

			FlowDocument doc = new FlowDocument();
			doc.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			Paragraph p = new Paragraph();

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				if (line == String.Empty)
				{
					continue;
				}

				string[] splitbytabs = line.Split("\t");
				string rline = splitbytabs[0].Substring(1);

				string path = splitbytabs[1];
				string host = splitbytabs[2];
				int port = int.Parse(splitbytabs[3]);

				char type = line[0];
				switch (type)
				{
					case '0':
						p.Inlines.Add(GenerateHyperlink(rline, host, port, path, "text"));
						break;
					case '1':
						p.Inlines.Add(GenerateHyperlink(rline, host, port, path, "gopher"));
						break;
					case 'i':
						p.Inlines.Add(new Run(rline));
						break;
					case 'h':
						p.Inlines.Add(new Run(rline));
						break;
					case 'd':
						p.Inlines.Add(new Run(rline));
						break;
					case 's':
						p.Inlines.Add(new Run(rline));
						break;
					default:
						p.Inlines.Add(new Run(rline));
						break;
				}
				if (i < lines.Length)
					p.Inlines.Add(new LineBreak());
			}

			doc.Blocks.Add(p);
			doc.PageWidth = 1000;
			return doc;
		}

		private static Hyperlink GenerateHyperlink(string text, string host, int port, string path, string type)
		{
			Hyperlink link = new Hyperlink(new Run(text));
			link.Command = MainViewModel.NavigateToUrlBehavior($"gopher://{host}:{port}{path}", type);
			return link;
		}
	}
}
