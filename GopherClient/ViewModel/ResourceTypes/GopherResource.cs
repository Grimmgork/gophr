using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Linq;
using GopherClient.Model.Protocols;
using GopherClient.Model;
using System.Windows;
using System.Windows.Input;
using GopherClient.Commands;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace GopherClient.ViewModel.ResourceTypes
{
	public class GopherResource : ResourceTypeBase
	{
		private static HashSet<char> supportedTypes = new HashSet<char>() { 'i', '0', '1', '3', '7', '9', 'g', 'I', 'h' };
		private StringBuilder unprocessedData = new StringBuilder();

		public ObservableCollection<GopherElement> Elements { get; set; } = new ObservableCollection<GopherElement>();
		public override void AppendData(byte[] chunk, bool isLastChunk)
		{
			base.AppendData(chunk, isLastChunk);
			unprocessedData.Append(Encoding.ASCII.GetString(chunk));
			InterpretData(isLastChunk);
		}

		int _selectedIndex = 0;
		public int SelectedIndex
        {
            get
            {
				return _selectedIndex;
            }
            set
            {
				_selectedIndex = 2;
				OnPropertyChanged("SelectedIndex");
				Trace.WriteLine("Property changed " + _selectedIndex.ToString());
				if(_selectedIndex != value)
                {
					if(Elements[value].type != 'i')
						_selectedIndex = value;
					OnPropertyChanged("SelectedIndex");
				}
            }
        }

		private void GoNextUp()
        {

        }

		private void GoNextDown()
        {

        }

		private void InterpretData(bool isLastChunk)
		{
			string[] rows = unprocessedData.ToString().Split("\r\n");
			for(int i = 0; i < rows.Length; i++)
            {
				string s = rows[i];
				GopherElement element = null;
				try
                {
					element = new GopherElement(s);
				}
                catch (Exception ex)
                {
					//if last row of chunk of data -> incomplete! wait for next chunk to complete
					if(i == rows.Length - 1)
                    {
						unprocessedData = new StringBuilder(s);
						break;
					}

					element = new GopherElement($"3{ex.Message}\t\t\t");
                }

				//point unsupported linux/dos/uuenc binary-files to just 'binary-file'
				if (element.type == '5' || element.type == '4' || element.type == '6')
					element.type = '9';

				//point unsupported types to 'unsupported'
				if (!IsTypeSupported(element.type))
					element.type = '.';

				Elements.Add(element);
			}
		}

		public bool IsTypeSupported(char t)
        {
			return supportedTypes.Contains(t);
        }
    }

	public class GopherElement
	{
		public string text { get; private set; }
		public string host { get; private set; }
		public int port { get; private set; }
		public string path { get; private set; }
		public char type { get; set; }

		public GopherElement(string row)
		{
			type = row[0];
			string[] columns = row.Split("\t");
			text = columns[0].Substring(1);
			path = columns[1];
			host = columns[2];
			port = 0;
			int rport;
			if(int.TryParse(columns[3],out rport))
            {
				port = rport;
			}
		}

		public GopherElement(char type, string text)
		{
			this.type = type;
			this.text = text;
		}
	}
}
