using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GopherClient.Model
{
    public class FileWriter : IDataChunkConsumer
    {
        private FileStream fs;

        public FileWriter(string filename)
        {
            fs = File.Create(filename);
        }

        private void AddChunk(byte[] chunk, bool lastone)
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

        public Task Consume(ResourceRequest request, CancellationToken t)
        {
            return Task.Run(() =>
            {
                byte[] chunk = null;
                while (!t.IsCancellationRequested)
                {
                    chunk = request.AwaitNextChunk(t).Result;
                    if (chunk == null)
                    {
                        AddChunk(chunk, true);
                        break;
                    }
                    AddChunk(chunk, false);
                }
                Dispose();
            });
        }
    }
}
