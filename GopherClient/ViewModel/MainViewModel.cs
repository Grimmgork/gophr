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
using System.Threading;

namespace GopherClient.ViewModel
{
	public class MainViewModel : OnPropertyChangedBase
	{
		private static MainViewModel mvm;
		private static CancellationTokenSource cancelTokenSource;
		private static Task<Resource> task;

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

		public ICommand RefreshCommand{
			get{
				return new RelayCommand(o => { Navigate(Location, null); }, o => true);
			}
		}

		private HistoryStack<string> History;

		private string _location;
		public string Location{
			get{
				return _location;
			}
			set{
				if(Location != value){
					_location = value;
					OnPropertyChanged("Location");
					Navigate(Location, null);
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

		private StatusState _status;
		public StatusState Status
		{
			get{
				return _status;
			}
			set{
				_status = value;
				OnPropertyChanged("Status");
			}
		}

		public enum StatusState{
			fetching,
			parsing,
			error,
			done
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

		public MainViewModel(){
			MainViewModel.mvm = this;

			ResourceFetcher.Init();
			History = new HistoryStack<string>(100);
			Location = "gopher://gopher.floodgap.com";
		}

		public async void Navigate(string url, string type)
		{
			Result = null;
			Status = StatusState.fetching;

			_location = url;
			OnPropertyChanged("Location");
			if(History.Value != url)
				History.Push(url);

			if(cancelTokenSource != null)
				cancelTokenSource.Cancel();

			cancelTokenSource = new CancellationTokenSource();
			CancellationToken t = cancelTokenSource.Token;
			task = Task.Factory.StartNew(() => {  return ResourceFetcher.Request(url, t); }, t);
			Trace.WriteLine("Start task!");	
			try
			{
				await task;
				Trace.WriteLine("Kek!");

				Resource request = task.Result;
				if (type == null)
					type = request.Type;

				Status = StatusState.parsing;
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

				Status = StatusState.done;
				Trace.WriteLine("Success!");
			}
			catch{
				Trace.WriteLine("Error!");
				Status = StatusState.error;
			}
		}

		public static ICommand NavigateToUrlBehavior(string url, string type){
			return new RelayCommand(o => { mvm.Navigate(url, type); }, o => true);
		}
	}
}
