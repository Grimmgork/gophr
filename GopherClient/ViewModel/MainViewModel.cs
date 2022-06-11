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
using GopherClient.Model.Protocols;
using System.Collections.ObjectModel;
using System.Windows.Controls;

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

		public ICommand LoadServerRootCommand
		{
			get
			{
				return new RelayCommand(o => { Navigate( GopherProtocol.GetServerMainPage(History.Value) ); }, o => true);
			}
		}

		public ICommand OneAboveCommand
		{
			get
			{
				return new RelayCommand(o => { Navigate(GopherProtocol.GetOneAbove(History.Value)); }, o => History.Value != GopherProtocol.GetServerMainPage(History.Value));
			}
		}


		private HistoryStack<string> History = new HistoryStack<string>(100);

		public string Url{
			get{
				return History.Value;
			}
			set{
				if(Url != value && value != string.Empty){
					Navigate(value);
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
				OnPropertyChanged("FormatedDataSize");
			}
		}

		public string FormatedDataSize
        {
            get
            {
				int oom = 0;
				for(int i = 0; i < 4; i++)
                {
					if(DataSize < MathF.Pow(1000, i))
                    {
						break;
                    }
					oom = i;
				}
				string appendix = "";
				switch(oom)
                {
					case 0:
						appendix = "b";
						break;
					case 1:
						appendix = "kb";
						break;
					case 2:
						appendix = "mb";
						break;
				}
				return $"{Math.Round(DataSize/MathF.Pow(1000, oom),0)} {appendix}";
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
			if (MainViewModel.mvm != null)
				throw new Exception("There can only be one MainViewModel instance!");

			MainViewModel.mvm = this;
			Configuration conf = Configuration.Load();
			Url = "gopher://gopher.floodgap.com";
		}

		public async void Navigate(string newLocation)
		{
			if (cancelTokenSource != null)
				cancelTokenSource.Cancel();

			Result = null;
			DataSize = 0;
			Status = StatusState.fetching;

			if(History.Value != newLocation)
				History.Push(newLocation);
			OnPropertyChanged("Url");

			cancelTokenSource = new CancellationTokenSource();
			CancellationToken t = cancelTokenSource.Token;

			GopherResourceType type = GopherResourceType.Unknown;
			Tuple<string, GopherResourceType> urlTypeExtraction = GopherProtocol.ExtractTypeFromUrl(newLocation);

			type = urlTypeExtraction.Item2;
			History.Value = GopherProtocol.EmbedTypeInUrl(urlTypeExtraction.Item1, type);
			OnPropertyChanged("Url");

			Uri finalUri = new Uri(urlTypeExtraction.Item1);

			ResourceRequest request = ResourceRequest.Request(finalUri, t);

			switch (type)
			{
				case GopherResourceType.Gopher:
					Result = new GopherResource();
					break;
				case GopherResourceType.Image:
					Result = new TextResource();
					break;
				default:
					Result = new TextResource();
					break;
			}

            while (!t.IsCancellationRequested)
            {

                try
                {
                    byte[] chunk = await request.AwaitNextChunk(t);
                    if (chunk == null)
                    {
                        Status = StatusState.parsing;
                        break;
                    }

                    Result.AppendData(chunk, false);
                    DataSize = request.Size;
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }

            Status = StatusState.done;
		}

		public static ICommand NavigateToUrlBehavior(string url){
			return new RelayCommand(o => { mvm.Navigate(url); }, o => true);
		}

		public static ICommand UpdateInfo(string text){
			return new RelayCommand(o => { mvm.Info = text; }, o => true);
		}
	}
}