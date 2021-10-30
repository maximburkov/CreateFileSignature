using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace CreateFileSignature
{
    class Program
    {
        static void GetSha256(byte[] bytes, int index)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var res = BitConverter.ToString(sha256.ComputeHash(bytes));
                var thread = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine($"#{index}, t: {thread}: {res}");
            }
        }

        static string GetSha256String(byte[] bytes)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return BitConverter.ToString(sha256.ComputeHash(bytes));
            }
        }

        static void Main(string[] args)
        {
            #region Generate text file

            //string path = "Test.txt";

            //using (StreamWriter sw = new StreamWriter(path))
            //{
            //    for (int i = 0; i < 100000000; i++)
            //    {
            //        sw.WriteLine($"New Line: {i}");
            //    }
            //}
            #endregion


            //#region Solution
            int chunkLength = 1024 * 1024 * 100;
            string filePath = "Test.txt";
            int threadsCount = Environment.ProcessorCount;
            
            int offset = 0;
            int index = 0;

            int bytesReaded;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //int threadCount = (int)(Environment.ProcessorCount);
            int threadCount = 5;

            using (WorkerPool pool = new WorkerPool(threadCount))
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                do
                {
                    byte[] chunk = new byte[chunkLength];
                    index++;
                    bytesReaded = fileStream.Read(chunk, offset, chunkLength);
                    var command = new PrintSignatureCommand(index, chunk);
                    pool.Enqueue(command);
                }
                while (bytesReaded >= chunkLength);

                //Thread.Sleep(5000);
                pool.Finsihwork();
            }

            sw.Stop();
            Console.WriteLine($"Time: {sw.Elapsed.TotalMilliseconds}");
        }
    }
}
