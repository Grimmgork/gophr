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
	public class GopherPageViewModel : OnPropertyChangedBase, IDataChunkConsumer
	{
		public ObservableCollection<GopherElement> Elements { get; set; } = new ObservableCollection<GopherElement>();

		private static HashSet<char> supportedTypes = new HashSet<char>() { 'i', '0', '1', '3', '7', '9', 'g', 'I', 'h' };
		private StringBuilder unprocessedData = new StringBuilder();


		public ICommand SelectDownCommand
        {
            get
            {
				return new RelayCommand(o => { SelectDown(); }, o => true);
			}
        }

		public ICommand SelectUpCommand
		{
			get
			{
				return new RelayCommand(o => { SelectUp(); }, o => true);
			}
		}

		public ICommand ExecuteSelectedElementCommand
        {
			get
            {
				return new RelayCommand(o => { Elements[SelectedIndex].Interact(); }, o => true);
            }
        }

		public ICommand SelectNextInteractableUpCommand
		{
			get
			{
				return new RelayCommand(o => {
					int index = FindNextInteractableIndex(true);
					if (index != -1)
						SelectedIndex = index;
				}, o => true);
			}
		}

		public ICommand SelectNextInteractableDownCommand
		{
			get
			{
				return new RelayCommand(o => {
					int index = FindNextInteractableIndex(false);
					if (index != -1)
						SelectedIndex = index;
				}, o => true);
			}
		}

		
		public void AddChunk(byte[] chunk, bool isLastChunk)
		{
			if(chunk != null)
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
				if(_selectedIndex != value)
                {
					int old = _selectedIndex;
					_selectedIndex = value;
					OnPropertyChanged("SelectedIndex");

					Elements[old].OnPropertyChanged("IsSelected");					
					SelectedElement.OnPropertyChanged("IsSelected");

					if (SelectedElement.IsInteractable)
						PushUrlToInfo(SelectedElement);
					else
						PushUrlToInfo(null);
				}
            }
        }

		public GopherElement SelectedElement
        {
            get
            {
				if(Elements.Count > 0)
					return Elements[SelectedIndex];
				return null;
            }
            set
            {
				if(SelectedElement != value)
                {
					SelectedIndex = value.index;
					OnPropertyChanged("SelectedElement");
				}
            }
        }

		public static void PushUrlToInfo(GopherElement element)
        {
			string url = "";
			if (element != null)
				url = element.url;

			MainViewModel.UpdateInfo(url).Execute(null);
		}

		private void SelectUp()
        {
			Trace.WriteLine("going up!");
			if(SelectedIndex > 0)
				SelectedIndex--;
        }

		private void SelectDown()
        {
			Trace.WriteLine("going down!");
			if(SelectedIndex < Elements.Count -1)
				SelectedIndex++;
		}

		private int FindNextInteractableIndex(bool up)
        {
			int increment = 1;
			if (up)
				increment = -1;

			int index = SelectedIndex + increment;
			while (index >= 0 && index < Elements.Count)
            {
				if (Elements[index].type != 'i')
					return index;

				index += increment;
            }

			return -1;
		}

		private void InterpretData(bool isLastChunk)
		{
			string[] rows = unprocessedData.ToString().Split("\r\n");
			for(int i = 0; i < rows.Length; i++)
            {
				string row = rows[i];
				if (row == String.Empty || row == ".")
					continue;

				GopherElement element = null;
				try
                {
					element = new GopherElement(row,this);
				}
                catch (Exception ex)
                {
					//if last row of chunk of data -> incomplete! wait for next chunk to complete
					if(i == rows.Length - 1 && !isLastChunk)
                    {
						unprocessedData = new StringBuilder(row);
						break;
					}

					element = new GopherElement($"3{ex.Message}\t\t\t", this);
                }

				//point unsupported linux/dos/uuenc binary-files to just 'binary-file'
				if (element.type == '5' || element.type == '4' || element.type == '6')
					element.type = '9';

				//point unsupported types to 'unsupported'
				if (!IsTypeSupported(element.type))
					element.type = '.';

				switch (element.type)
				{
					case '1':
						element.SetInteraction(MainViewModel.NavigateToUrlBehavior(element.url));
						break;
					case '0':
						element.SetInteraction(MainViewModel.NavigateToUrlBehavior(element.url));
						break;
				}

				Elements.Add(element);
				element.index = Elements.Count - 1;
			}
		}

		public bool IsTypeSupported(char t)
        {
			return supportedTypes.Contains(t);
        }

        public void Dispose()
        {
            
        }
    }

	public class GopherElement : OnPropertyChangedBase
	{
		public string text { get; private set; }
		public string host { get; private set; }
		public int port { get; private set; }
		public string path { get; private set; }
		public char type { get; set; }

		public int index { get; set; }
		public string url { get; private set; }

		private ICommand interaction;
		private GopherPageViewModel parent;

		public bool IsSelected
        {
            get
            {
				return parent.SelectedIndex == index;
            }
        }

		public bool IsInteractable
        {
            get
            {
				return interaction != null;
            }
        }

		public GopherElement(string row, GopherPageViewModel parent)
		{
			this.parent = parent;
			type = row[0];
			string[] columns = row.Split("\t");
			text = columns[0].Substring(1);
			path = columns[1];
			host = columns[2];
			port = -1;
			int rport;

			if(int.TryParse(columns[3],out rport))
            {
				port = rport;
			}

            try
            {
				url = GopherProtocol.GenrateUrl(host, path, type, port);
			}
            catch
            {

            }
		}

		public void SetInteraction(ICommand interaction)
        {
			this.interaction = interaction;
        }

		public void Interact()
        {
			if(interaction != null)
				interaction.Execute(null);
        }
	}
}
