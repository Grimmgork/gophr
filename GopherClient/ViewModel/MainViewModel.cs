using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using System.IO;
using System.Windows;
using System.Linq;

using GopherClient.Model;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Threading;

namespace GopherClient.ViewModel
{
	public class MainViewModel : OnPropertyChangedBase
	{
		public ICommand BackCommand{
			get{
				return new RelayCommand(o => { Navigate(History.Previous()); }, o => History.HasPrevious);
			}
		}

		public ICommand ForwardCommand
		{
			get
			{
				return new RelayCommand(o => { Navigate(History.Next()); }, o => History.HasNext);
			}
		}

		private HistoryStack<string> History;

		private string _location;
		public string Location{
			get{
				return _location;
			}
			set{
				if(value != _location){
					_location = value;
					OnPropertyChanged("Location");
					Navigate(Location);
					if(History.Value != Location)
						History.Push(Location);
				}
			}
		}

		private string _status;
		public string Status{
			get{
				return _status;
			}
			set{
				_status = value;
				OnPropertyChanged("Status");
			}
		}

		private FlowDocument _doc;
		public FlowDocument Doc{
			get{
				return _doc;
			}
			set{
				_doc = value;
				OnPropertyChanged("Doc");
			}
		}

		public MainViewModel(){
			ResourceRequester.Init();
			History = new HistoryStack<string>(100);

			Location = "file:///c:/users/eric/desktop/floodgap.txt";
		}

		public async void Navigate(string url)
		{
			Doc = null;
			Location = url;
			Status = "fetching ...";

			try{
				byte[] data = await Task.Run(() => {
					return ResourceRequester.GetResource(new Uri(url));
				});
				Status = "Parsing ...";
				Doc = GenerateGopherDocument(data);
				Status = "Done!";
			}
			catch{
				Doc = GenerateErrorPage(1);
				Status = "Error!";
			}
		}

		public FlowDocument GenerateGopherDocument(byte[] data)
		{
			string text = Encoding.ASCII.GetString(data);
			string[] lines = text.Split("\n");

			FlowDocument doc = new FlowDocument();
			doc.PageWidth = 2500;
			doc.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			Paragraph p = new Paragraph();

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				if(line == String.Empty){
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
					case '1':
						Hyperlink link = new Hyperlink(new Run(rline));
						string newurl = $"gopher://{host}:{port}{path}";
						link.Command = new RelayCommand(o => { Location = newurl; }, o => true);
						p.Inlines.Add(link);
						break;
					default:
						p.Inlines.Add(new Run(rline));
						break;
				}
				if (i < lines.Length)
					p.Inlines.Add(new LineBreak());
			}

			doc.Blocks.Add(p);
			return doc;
		}

		public FlowDocument GenerateErrorPage(int errorcode){
			FlowDocument doc = new FlowDocument();
			Paragraph p = new Paragraph();
			p.Inlines.Add(new Run("Error!"));
			doc.Blocks.Add(p);
			return doc;
		}

		public class RelayCommand : ICommand
		{
			private Action<object> execute;
			private Func<object, bool> canExecute;

			public event EventHandler CanExecuteChanged
			{
				add { CommandManager.RequerySuggested += value; }
				remove { CommandManager.RequerySuggested -= value; }
			}

			public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
			{
				this.execute = execute;
				this.canExecute = canExecute;
			}

			public bool CanExecute(object parameter)
			{
				return this.canExecute == null || this.canExecute(parameter);
			}

			public void Execute(object parameter)
			{
				this.execute(parameter);
			}
		}
	}
}
