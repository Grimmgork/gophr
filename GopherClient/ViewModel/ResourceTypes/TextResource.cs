using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;

namespace GopherClient.ViewModel.ResourceTypes
{
	public class TextResource : ResourceTypeBase
	{
		FlowDocument _doc = new FlowDocument() { PageWidth = 1000 };
		public FlowDocument Doc{
			get{
				return _doc;
			}
			set{
				_doc = value;
				OnPropertyChanged("Doc");
			}
		}

		public override void AppendData(byte[] chunk, bool lastOne)
		{
			base.AppendData(chunk, lastOne);
			Paragraph p = new Paragraph();
			p.Inlines.Add(new Run(Encoding.ASCII.GetString(chunk)));
			Doc.Blocks.Add(p);
			Trace.WriteLine(Doc.Blocks.Count);
		}
	}
}