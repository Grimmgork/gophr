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
using System.Threading.Tasks;
using System.Threading;

namespace GopherClient.ViewModel.BrowserPages
{
	public class GopherPageViewModel : BrowserPageBase
	{
		public ObservableCollection<GopherElement> Elements { get; set; } = new ObservableCollection<GopherElement>();

		private static HashSet<char> supportedTypes = new HashSet<char>() { 'i', '0', '1', '3', '7', '9', 'g', 'I', 'h' };

		public GopherPageInterpreter interpreter = new GopherPageInterpreter();

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

		public ICommand DownloadSelectedElementToDiskCommand
		{
			get
			{
				return new RelayCommand(o => {
					DownloadElementToDisk(SelectedElement);
				}, o => true);
			}
		}

		public ICommand Nothin
        {
            get
            {
				return new RelayCommand(o => { }, o => true);
            }
        }

		private void AddChunk(byte[] chunk, bool isLastChunk)
		{
			string text = "";
			if (chunk != null)
            {
				text = Encoding.ASCII.GetString(chunk);
				InterpretData(text, isLastChunk);
			}
				
			//Trace.WriteLine($"[CHUNK START]{text}[CHUNK END]");
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
						PushInfo(SelectedElement);
					else
						PushInfo(null);
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

		public static void PushInfo(GopherElement element)
        {
			string info = "";
			if (element != null)
				info = $"{element.type} - {element.url}";

			MainViewModel.UpdateInfo(info).Execute(null);
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

		public void DownloadElementToDisk(GopherElement e)
        {
			string url = e.url;
			GopherUrl gurl = new GopherUrl(url);
			gurl.Type = '9';

			MainViewModel.NavigateToUrlBehavior(gurl.ToString()).Execute(null);
        }

		private void InterpretData(string newdata, bool isLastChunk)
		{
			string[] rows = interpreter.Interpret(newdata);
			for(int i = 0; i < rows.Length; i++)
            {
				string row = rows[i];
				if (row == String.Empty || row == ".")
					continue;

				GopherElement element = new GopherElement(row, this);

				//point unsupported linux/dos/uuenc binary-files to just 'binary-file' --> download type?
				if (element.type == '5' || element.type == '4' || element.type == '6')
					element.type = '9';

				//point a png type to Image
				if (element.type == 'p')
					element.type = 'I';

				//point unsupported types to 'unsupported'
				if (!IsTypeSupported(element.type))
					element.type = '.';

				if(element.type != 'i' && element.type != '.')
					element.SetInteraction(MainViewModel.NavigateToUrlBehavior(element.url));

				Elements.Add(element);
				element.index = Elements.Count - 1;
			}
		}

		public bool IsTypeSupported(char t)
        {
			return supportedTypes.Contains(t);
        }

        public override async Task Consume(ResourceRequest request, CancellationToken t)
        {
			while (!t.IsCancellationRequested)
			{
				try
				{
					await Task.Run(() => Thread.Sleep(5));
					byte[] chunk = await request.AwaitNextChunk(t);
					if (chunk == null)
					{
						AddChunk(chunk, true);
						break;
					}

					AddChunk(chunk, false);
					DataSize = request.Size;
				}
				catch (OperationCanceledException)
				{
					break;
				}
			}
		}
    }

	public class GopherPageInterpreter
    {
		StringBuilder unprocessed = new StringBuilder();
		public string[] Interpret(string text)
        {
			List<string> result = new List<string>();
			unprocessed.Append(text);
			string u = unprocessed.ToString();
			int index = u.IndexOf("\r\n");
			while(index != -1)
            {
				string row = u.Substring(0, index);
				result.Add(row);
				u = new String(u.Skip(index + 2).ToArray());
				index = u.IndexOf("\r\n");
			}

			unprocessed = new StringBuilder(u);

			return result.ToArray();
        }
    }

	public class GopherElement : OnPropertyChangedBase
	{
		public string source { get; private set; }
		public string text { get; private set; }
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
			this.source = row;
			this.parent = parent;
			GopherUrl u = new GopherUrl();

			string[] columns = row.Split("\t");
			text = columns[0].Substring(1);
			string path = columns[1];

			u.Scheme = "gopher";
			u.Host = columns[2];
			u.Type = row[0];
			this.type = u.Type;

			string[] split = path.Split("URL:");
			u.Segments = split[0].Split("/").Where(a => a != "").ToArray();

			int p = -1;
			int.TryParse(columns[3], out p);
			if (p != -1)
				u.Port = p;
			
			if (u.Type == 'h')
            {
				if (split.Length > 1)
                {
					url = split[1];
					return;
				}
            }

			url = u.ToString();
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
