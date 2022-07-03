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

        public Task Consume(ResourceRequest request, CancellationToken t)
        {
            return Task.Run(() =>
            {
                byte[] chunk = null;
                while (!t.IsCancellationRequested)
                {
                    chunk = request.AwaitNextChunk(t).Result;
                    if (chunk == null)
                        break;

                    fs.Write(chunk);
                }

                fs.Close();
            });
        }
    }
}
