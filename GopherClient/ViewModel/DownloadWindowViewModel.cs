using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using GopherClient.View;

namespace GopherClient.ViewModel
{
    public class DownloadWindowViewModel : OnPropertyChangedBase
    {
        private string tempDirectory;
        private DownloadsWindow View;

        public ObservableCollection<DownloadEntry> Downloads;

        public DownloadWindowViewModel()
        {
            tempDirectory = Path.GetTempPath() + @"\kek";
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);
        }

        public void Show()
        {
            if (View == null)
                View = new DownloadsWindow();

            View.Show();
        }

        public void Close()
        {
            if(View != null)
                View.Close();
            View = null;
        }

        public void StartDownload(string url)
        {

        }

        public string StartTempDownload()
        {
            string file = GenerateNewTempFilePath();
            return file;
        }

        public void DisposeTempDownload(string id)
        {

        }

        public void DisposeAllTempDownloads()
        {

        }

        public void AsRealDownload(string path, string id)
        {

        }

        private string GenerateNewTempFilePath()
        {
            return tempDirectory + new Guid().ToString();
        }
    }

    public class DownloadEntry : OnPropertyChangedBase
    { 
        public string url { get; set; }
        public bool IsFinished { get; }

        public string Id { get; private set; }

        public DownloadEntry(string url, string filePath)
        {
            this.url = url;
        }
    }
}
