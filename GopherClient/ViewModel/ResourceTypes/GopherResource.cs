using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Linq;

namespace GopherClient.ViewModel.ResourceTypes
{
	public class GopherResource : ResourceTypeBase
	{
		private string unprocessedData = "";

		FlowDocument _doc = new FlowDocument() { PageWidth = 10000, FontFamily = new System.Windows.Media.FontFamily("Consolas"), Foreground = Brushes.WhiteSmoke, LineHeight = 1, PagePadding = new System.Windows.Thickness(10d) };
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

		public override void AppendData(byte[] chunk, bool isLastChunk)
		{
			base.AppendData(chunk, isLastChunk);
			unprocessedData += Encoding.ASCII.GetString(chunk);

			InterpretData(isLastChunk);
		}

		private void InterpretData(bool isLastChunk)
		{
			StringBuilder builder = new StringBuilder(unprocessedData);
			List<GopherElement> elements = new List<GopherElement>();
			string[] rows = builder.ToString().Split("\r\n");
			int startindex = 0;

			for (int i = 0; i < rows.Length; i++)
            {
				int length = rows[i].Length;
				try
                {
					elements.Add(new GopherElement(rows[i]));
					builder.Remove(startindex, length);
				}
                catch(Exception e)
                {
					if(i == rows.Length - 1 && isLastChunk){
						elements.Add(new GopherElement('3', e.Message));
						builder.Clear();
                    }
                }
				startindex += length+3;
            }

			unprocessedData = builder.ToString();

			foreach (GopherElement element in elements){
				AppendElementToPage(element);
			}
		}

		public void AppendElementToPage(GopherElement element)
        {
			Doc.Blocks.Add(new Paragraph(ConstructInline(element)));
        }

		public static Inline ConstructInline(GopherElement row)
        {
			switch (row.type)
			{
				case '0':
					return GenerateHyperlink(row.text, row.host, row.port, row.path, ResourceType.Text);
				case '1':
					return GenerateHyperlink(row.text, row.host, row.port, row.path, ResourceType.Gopher);
				case '3':
					return new Run(row.text) { Foreground = Brushes.Red };
				case 'i':
					return new Run(row.text);
				case 'h':
					return new Run(row.text);
				case 'd':
					return new Run(row.text);
				case 's':
					return new Run(row.text);
				default:
					return new Run(row.text);
			}
		}

		private static Hyperlink GenerateHyperlink(string text, string host, int port, string path, ResourceType type)
		{
			Hyperlink link = new Hyperlink(new Run(text));
			string url = $"gopher://{host}:{port}/{ String.Join("/", path.Split("/").Where(s => s != string.Empty)) }";
			link.ToolTip = url;
			link.Command = MainViewModel.NavigateToUrlBehavior(url, type);
			return link;
		}
	}

	public class GopherElement
	{
		public readonly string text;
		public readonly string host;
		public readonly int port;
		public readonly string path;
		public readonly char type;

		public GopherElement(string row){
				type = row[0];
				string[] columns = row.Split("\t");
				text = columns[0].Substring(1);
				path = columns[1];
				host = columns[2];
				port = int.Parse(columns[3]);
		}

		public GopherElement(char type, string text)
        {
			this.type = type;
			this.text = text;
        }
	}
}
