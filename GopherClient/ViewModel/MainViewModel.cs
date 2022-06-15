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
using Microsoft.Win32;
using GopherClient.View;
using static GopherClient.Model.Protocols.GopherProtocol;

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
				return new RelayCommand(o => { Navigate(new GopherUrl(History.Value).GetServerRoot().ToString()); }, o => true);
			}
		}
		public ICommand OneAboveCommand
		{
			get
			{
				return new RelayCommand(o => { Navigate(new GopherUrl(History.Value).GetOneAbove().ToString() ); }, o => History.Value != new GopherUrl(History.Value).GetServerRoot().ToString());
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
				Navigate(value);
				OnPropertyChanged("Url");
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
			switch (e.PropertyName)
            {
				case "DataSize":
					this.DataSize = (sender as BrowserPageBase).DataSize;
					break;
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
			Uri url = new Uri(newLocation);
            switch (url.Scheme)
            {
				case "gopher":
					NavigateGopher(new GopherUrl(newLocation));
					break;
				case "http":
					NavigateGopher(new GopherUrl(newLocation));
					break;
				case "https":
					NavigateGopher(new GopherUrl(newLocation));
					break;
				default:
					MessageBox.Show("Protocol not supported!");
					break;
            }
		}

		public void NavigateGopher(GopherUrl gurl)
        {
			if (History.Value != gurl.ToString())
				History.Push(gurl.ToString());
			OnPropertyChanged("Url");

			char type = gurl.Type;
			Uri urlWithoutType = new Uri(gurl.UrlWithoutType());
			switch (type)
			{
				case '1':
					CancellationToken t = NewPageCancelToken();
					NewPage(new GopherPageViewModel(), ResourceRequestFactory.NewRequest(urlWithoutType, t), t);
					break;
				case '7':
					if (gurl.Query == String.Empty)
					{
						QueryPrompt p = new QueryPrompt();
						p.ShowDialog();
						gurl.Query = p.Result;

						History.Pop();
						History.Push(gurl.ToString());
						OnPropertyChanged("Url");
					}

					CancellationToken tt = NewPageCancelToken();
					NewPage(new GopherPageViewModel(), ResourceRequestFactory.NewRequest(new Uri(gurl.ToString()), tt), tt);
					break;
				case '9':
					OpenFileDialog d = new OpenFileDialog();
					d.ShowDialog();

					if (d.FileName != String.Empty)
						DownloadToFile(d.FileName, urlWithoutType);

					History.Pop();
					OnPropertyChanged("Url");
					break;
				case 'h':

					break;
				default:
					OpenInExternalApplication(urlWithoutType, type);
					History.Pop();
					OnPropertyChanged("Url");
					break;
			}
		}

		public async void NewPage(BrowserPageBase consumer, ResourceRequest request, CancellationToken t)
        {
			DataSize = 0;
			Result = consumer;
			Status = StatusState.fetching;

			await consumer.Consume(request, t);

			consumer.Dispose();
			Status = StatusState.done;
		}

		public CancellationToken NewPageCancelToken()
        {
			if (cancelTokenSource != null)
				cancelTokenSource.Cancel();

			cancelTokenSource = new CancellationTokenSource();
			return cancelTokenSource.Token;
		}

		public async void OpenInExternalApplication(Uri url, char type)
		{
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
			CancellationToken t = new CancellationTokenSource().Token;
			return new FileWriter(filePath).Consume( ResourceRequestFactory.NewRequest(url, t), t);
		}

		public static ICommand NavigateToUrlBehavior(string url){
			return new RelayCommand(o => { mvm.Navigate(url); }, o => true);
		}
		public static ICommand UpdateInfo(string text){
			return new RelayCommand(o => { mvm.Info = text; }, o => true);
		}
	}

	public class ExternalAppCommand
    {
		public string type;
		public string app;
		public string arguments;
    }
}