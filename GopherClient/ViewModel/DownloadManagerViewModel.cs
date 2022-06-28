using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using GopherClient.View;

namespace GopherClient.ViewModel
{
    public class DownloadManagerViewModel : OnPropertyChangedBase
    {
        private string tempDirectory;

        public ObservableCollection<DownloadEntry> Downloads;

        public DownloadManagerViewModel()
        {
            tempDirectory = Path.GetTempPath() + @"\kek";
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);
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
