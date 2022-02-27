using System;
using System.Collections.Generic;
using System.Text;

namespace GopherClient.ViewModel.ResourceTypes
{
	public class TextResource : ResourceTypeBase
	{
		string _text;
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}

		public TextResource(byte[] data)
		{
			Text = Encoding.ASCII.GetString(data);
		}
	}
}
