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

namespace GopherClient.ViewModel
{
	public class MainViewModel : OnPropertyChangedBase
	{
		private static MainViewModel mvm;
		private static CancellationTokenSource cancelTokenSource;
		private static string[] Args;

		private Configuration config;

		private DownloadManagerViewModel Downloads;

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
				return new RelayCommand(o => { }, o => false);
			}
		}

		public ICommand OneAboveCommand
		{
			get
			{
				return new RelayCommand(o => { }, o => false);
			}
		}

		public ICommand CloseDownloadManagerCommand
		{
			get
			{
				return new RelayCommand(o => {  }, o => true);
			}
		}

		public ICommand ShowDownloadManagerCommand
        {
			get
			{
				return new RelayCommand(o => {  }, o => true);
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
			config = Configuration.Load();
			Configuration.Save(config);

			string starturl = config.startUrl;
			if (Args.Length > 0)
				starturl = Args[0];

			Navigate("gopher://gopher.floodgap.com");
		}

		public void Navigate(string newLocation, bool writeToHistory = true)
		{
			GopherUrl url = new GopherUrl(newLocation);

			if (url.Scheme != "gopher")
            {
				if (config.trustetProtocols.Contains(url.Scheme))
					OpenUrl(url.ToString(false));
                else
                {
					MessageBox.Show($"Cant resolve unsafe protocol '{url.Scheme}:', Copied url to clipboard!");
					Clipboard.SetText(url.ToString(false));
				}

				return;
			}

			if (url.Type == '7' && url.Query == "")
			{
				QueryPrompt p = new QueryPrompt();
				p.ShowDialog();
				if (p.Result == "")
					return;

				url.Query = p.Result;
			}

			CancellationToken t = NewPageCancelToken();
			DisplayResource(ResourceRequestFactory.NewRequest(url, t), t);
		}

		public async void DisplayResource(ResourceRequest req, CancellationToken t)
		{
			Status = StatusState.fetching;

			string mimetype = await req.AwaitMimeType(t);
            switch (mimetype)
            {
				case "text/gopher":
					PushHistory(req.Url.ToString());
					NewPage(new GopherPageViewModel(), req, t);
					break;
				default:
					DisplayFileExternal(req, t);
					break;
			}

			Status = StatusState.done;
		}

		public void PushHistory(string url)
        {
			if (History.Value != url)
				History.Push(url);

			OnPropertyChanged("Url");
		}

		public async void NewPage(BrowserPageBase consumer, ResourceRequest request, CancellationToken t)
        {
			DataSize = 0;
			Result = consumer;
			await consumer.Consume(request, t);
			consumer.Dispose();
		}

		public CancellationToken NewPageCancelToken()
        {
			if (cancelTokenSource != null)
				cancelTokenSource.Cancel();

			cancelTokenSource = new CancellationTokenSource();
			return cancelTokenSource.Token;
		}

		public async void DisplayFileExternal(ResourceRequest req, CancellationToken t)
		{
			string type = await req.AwaitMimeType(t);

            if (config.typeMappings.ContainsKey(type))
            {
				string file = await DownloadToTempFile(req);
				RunOpenAppCommand(type, file);
				return;
            }

            if (config.trustedFileExtensions.Contains(req.Url.FileExtension))
            {
				string file = await DownloadToTempFile(req);
				OpenUrl(file);
				return;
            }

			//download to file permanently
			SaveFileDialog d = new SaveFileDialog();
			d.ShowDialog();
			if (d.FileName == "")
				return;

			await DownloadToFile(d.FileName, req);
		}

		public static void OpenUrl(string path)
        {
			var ps = new ProcessStartInfo(path)
			{
				UseShellExecute = true,
				Verb = "open"
			};
			Process.Start(ps);
		}

		public void RunOpenAppCommand(string mimeType, string filePath)
        {
			string batch = config.typeMappings[mimeType];
			System.Diagnostics.Process.Start(batch, $"\"{filePath}\"");
		}

		public Task DownloadToFile(string filePath, ResourceRequest req, bool temp = true)
		{
			CancellationToken t = new CancellationTokenSource().Token;
			return new FileWriter(filePath).Consume(req, t);
		}

		public Task<string> DownloadToTempFile(ResourceRequest req)
        {
			return Task.Run<string>(() => {
				string filePath = $"{Path.GetTempPath()}gophr\\{Guid.NewGuid()}\\{req.Url.Segments.Last()}";
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
				DownloadToFile(filePath, req).Wait();
				return filePath;
			});
        }

		public static ICommand NavigateToUrlBehavior(string url){
			return new RelayCommand(o => { mvm.Navigate(url); }, o => true);
		}
		public static ICommand UpdateInfo(string text){
			return new RelayCommand(o => { mvm.Info = text; }, o => true);
		}

		public static void SetArguments(string[] args)
        {
			Args = args;
        }
	}
}