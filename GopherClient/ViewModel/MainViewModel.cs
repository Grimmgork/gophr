using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using System.IO;
using System.Windows;
using System.Linq;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Threading;

using GopherClient.Model;
using GopherClient.Commands;
using GopherClient.ViewModel.ResourceTypes;



namespace GopherClient.ViewModel
{
	public class MainViewModel : OnPropertyChangedBase
	{
		private static MainViewModel mvm;

		public ICommand BackCommand{
			get{
				return new RelayCommand(o => { Navigate(History.Previous(), null); }, o => History.HasPrevious);
			}
		}
		public ICommand ForwardCommand
		{
			get
			{
				return new RelayCommand(o => { Navigate(History.Next(), null); }, o => History.HasNext);
			}
		}

		private HistoryStack<string> History;

		public static string Kek = "lel";

		private string _location;
		public string Location{
			get{
				return _location;
			}
			set{
				if(value != _location){
					_location = value;
					OnPropertyChanged("Location");
					Navigate(Location, null);
					if(History.Value != Location)
						History.Push(Location);
				}
			}
		}

		private string _info;
		public string Info{
			get{
				return _info;
			}
			set{
				_info = value;
				OnPropertyChanged("Info");
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

		private ResourceTypeBase _result;
		public ResourceTypeBase Result{
			get{
				return _result;
			}
			set{
				_result = value;
				OnPropertyChanged("Result");
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
			MainViewModel.mvm = this;

			ResourceFetcher.Init();
			History = new HistoryStack<string>(100);
			Location = "gopher://gopher.floodgap.com";
		}

		public async void Navigate(string url, string type)
		{
			Result = null;
			Location = url;
			Status = "fetching ...";
			Trace.WriteLine(type);
			try
			{
				Resource request = await Task.Run(() => {
					return ResourceFetcher.Request(url);
				});

				if (type == null)
					type = request.Type;

				Status = "Parsing ...";
				switch(type){
					case "gopher":
						Result = new GopherResource(request.Data);
						break;
					case "image":
						Result = new TextResource(request.Data);
						break;
					default:
						Result = new TextResource(request.Data);
						break;
				}

				Status = "Done!";
				Trace.WriteLine("Success!");
			}
			catch{
				Trace.WriteLine("Error!");
				Doc = GenerateErrorPage(1);
				Status = "Error!";
			}
		}

		public static ICommand NavigateToUrlBehavior(string url, string type){
			return new RelayCommand(o => { mvm.Navigate(url, type); }, o => true);
		}

		public FlowDocument GenerateErrorPage(int errorcode){
			FlowDocument doc = new FlowDocument();
			Paragraph p = new Paragraph();
			p.Inlines.Add(new Run("Error!"));
			doc.Blocks.Add(p);
			return doc;
		}
	}
}
