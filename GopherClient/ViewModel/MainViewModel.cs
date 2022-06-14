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
using GopherClient.ViewModel.BrowserPages;
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

		private DownloadWindowViewModel Downloads;

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
		public ICommand CloseDownloadManagerCommand
		{
			get
			{
				return new RelayCommand(o => { Downloads.Close(); }, o => true);
			}
		}

		public ICommand ShowDownloadManagerCommand
        {
			get
			{
				return new RelayCommand(o => { Downloads.Close(); Downloads.Show(); }, o => true);
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
					OnPropertyChanged("Url");
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
					case 3:
						appendix = "gb";
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


		private BrowserPageBase _result;
		public BrowserPageBase Result
		{
			get{
				return _result;
			}
			set{
				_result = value;
                Result.PropertyChanged += Result_PropertyChanged;
				OnPropertyChanged("Result");
			}
		}

        private void Result_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "DataSize")
            {
				this.DataSize = (sender as BrowserPageBase).DataSize;
            }
        }

        public MainViewModel(){
			if (MainViewModel.mvm != null)
				throw new Exception("There can only be one MainViewModel instance!");

			MainViewModel.mvm = this;
			Configuration conf = Configuration.Load();

			Downloads = new DownloadWindowViewModel();

			Navigate("gopher://gopher.floodgap.com");
		}

		public void Navigate(string newLocation)
		{
			var typeExtraction = GopherProtocol.ExtractTypeFromUrl(newLocation);
			char type = typeExtraction.Item2;
			Uri realUrl = new Uri(typeExtraction.Item1);

			string fullUrl = GopherProtocol.EmbedTypeInUrl(realUrl.ToString(), type);
			if (History.Value != fullUrl)
				History.Push(fullUrl);

			OnPropertyChanged("Url");
			switch (type)
			{
				case '1':
					NewPage(new GopherPageViewModel(), realUrl);
					break;
				case '9':
					NewPage(new GopherPageViewModel(), realUrl);
					break;
				case '.':
					NewPage(new GopherPageViewModel(), realUrl);
					break;
				default:
					OpenInExternalApplication(realUrl, type);
					break;
			}
		}

		public async void NewPage(BrowserPageBase consumer, Uri url)
        {
			if (cancelTokenSource != null)
				cancelTokenSource.Cancel();

			cancelTokenSource = new CancellationTokenSource();
			CancellationToken t = cancelTokenSource.Token;

			ResourceRequest request = ResourceRequestFactory.NewRequest(url, t);

			DataSize = 0;
			Result = consumer;
			Status = StatusState.fetching;

			await consumer.Consume(request, t);

			consumer.Dispose();
			Status = StatusState.done;
		}

		public async void OpenInExternalApplication(Uri url, char type)
		{
			if(type == 'h')
            {
				History.Pop();
				OnPropertyChanged("Url");
				string u = new String(url.AbsolutePath.Skip(5).ToArray());
				OpenFileWithDefaultApp(u);
				return;
            }

			Status = StatusState.fetching;
			Directory.CreateDirectory($"{Path.GetTempPath()}gophr");
			string filePath = $"{Path.GetTempPath()}gophr/{url.AbsolutePath.Split("/").Last()}";

			await DownloadToFile(filePath, url);
			Status = StatusState.done;
			OpenFileWithDefaultApp(filePath);
		}

		public static void OpenFileWithDefaultApp(string path)
        {
			var ps = new ProcessStartInfo(path)
			{
				UseShellExecute = true,
				Verb = "open"
			};
			Process.Start(ps);
		}

		public Task DownloadToFile(string filePath, Uri url)
		{
			cancelTokenSource = new CancellationTokenSource();
			CancellationToken t = cancelTokenSource.Token;
			return StartBackgroundRequest(new FileWriter(filePath), url, t);
		}

		public Task StartBackgroundRequest(IDataChunkConsumer consumer, Uri url, CancellationToken t)
        {
			History.Pop();
			OnPropertyChanged("Url");

			ResourceRequest request = ResourceRequestFactory.NewRequest(url, t);
			return consumer.Consume(request, t);
		}

		public static ICommand NavigateToUrlBehavior(string url){
			return new RelayCommand(o => { mvm.Navigate(url); }, o => true);
		}

		public static ICommand UpdateInfo(string text){
			return new RelayCommand(o => { mvm.Info = text; }, o => true);
		}
	}
}