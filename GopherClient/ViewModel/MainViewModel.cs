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

		public ICommand BackCommand{
			get
			{
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
		public ICommand RefreshCommand{
			get
			{
				return new RelayCommand(o => { Navigate(History.Value); }, o => true);
			}
		}


		private HistoryStack<HistoryEntry> History = new HistoryStack<HistoryEntry>(100);

		public string Url{
			get{
				Trace.WriteLine("get URL string");
				return History.Value.url;
			}
			set{
				if(Url != value && value != string.Empty){
					Navigate(new HistoryEntry() { url=value, type=ResourceType.Unknown });
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

		private int _dataSize;
		public int DataSize{
			get{
				return _dataSize;
			}
			set{
				_dataSize = value;
				OnPropertyChanged("DataSize");
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
			Configuration conf = Configuration.Load();
			Url = "gopher://gopher.floodgap.com";
		}

		public async void Navigate(HistoryEntry newLocation)
		{
			Result = null;
			DataSize = 0;
			Status = StatusState.fetching;

			Uri uri = new Uri(newLocation.url);

			if(History.Value != newLocation)
				History.Push(newLocation);
			OnPropertyChanged("Url");

			if (cancelTokenSource != null)
				cancelTokenSource.Cancel();

			cancelTokenSource = new CancellationTokenSource();
			CancellationToken t = cancelTokenSource.Token;

            ResourceRequest request = ResourceRequest.Request(uri, t);
			
			if(History.Value.type == ResourceType.Unknown){
				History.Value = new HistoryEntry() { url = History.Value.url, type = await request.AwaitType(t) };
            }

			switch (History.Value.type)
			{
				case ResourceType.Gopher:
					Result = new GopherResource();
					break;
				case ResourceType.Image:
					Result = new TextResource();
					break;
				default:
					Result = new TextResource();
					break;
			}

			while (!t.IsCancellationRequested)
			{
				byte[] chunks = null;
				try{
					chunks = await request.AwaitChunk(t);
				}
				catch(OperationCanceledException){
					return;
				}
					
				if (chunks == null)
					break;

				Result.AppendData(chunks, false);
				DataSize = request.Size;
			}

			Status = StatusState.done;
			DataSize = request.Size;
		}

		public static ICommand NavigateToUrlBehavior(string url, ResourceType type){
			return new RelayCommand(o => { mvm.Navigate(new HistoryEntry() { url=url, type=type }); }, o => true);
		}
	}

	public struct HistoryEntry
    {
		public string url { get; set; }
		public ResourceType type { get; set; }

		public static bool operator ==(HistoryEntry a, HistoryEntry b)
		{
			return a.url == b.url && a.type == b.type;
		}

		public static bool operator !=(HistoryEntry a, HistoryEntry b)
		{
			return a.url != b.url || a.type != b.type;
		}

        public override string ToString()
        {
			return $"Entry: {url}, {type}";
        }

    }
}