using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GopherClient.Model
{
    public class FileWriter : IDataChunkConsumer
    {
        private FileStream fs;

        public FileWriter(string filename)
        {
            fs = File.Create(filename);
        }

        public void AddChunk(byte[] chunk, bool lastone)
        {
            if (lastone)
            {
                if(chunk != null)
                    fs.Write(chunk);
                fs.Close();
                return;
            }
                
            fs.Write(chunk);
        }

        public void Dispose()
        {
            fs.Close();
        }
    }

    public interface IDataChunkConsumer : IDisposable
    {
        public void AddChunk(byte[] chunk, bool lastone);
    }
}
