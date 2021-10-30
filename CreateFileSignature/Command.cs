using System;
using System.Security.Cryptography;
using System.Threading;

namespace CreateFileSignature
{
    // TODO: rename file

    /// <summary>
    /// TODO
    /// </summary>
    public class PrintSignatureCommand : ICommand
    {
        public static int ChunksHandled = 0;
        private int index;
        private byte[] bytes;

        public int Index => index;

        public PrintSignatureCommand(int index, byte[] bytes)
        {
            this.index = index;
            this.bytes = bytes;
        }

        /// <summary>
        /// Executes 
        /// </summary>
        public void Execute()
        {
            Console.WriteLine($"Started calculation for #{index}.");
            using (SHA256 sha256 = SHA256.Create())
            {
                var signature = BitConverter.ToString(sha256.ComputeHash(bytes));
                //var thread = Thread.CurrentThread.ManagedThreadId;
                //Console.WriteLine($"#{Index}, t: {thread}: {res}");
                //Thread.Sleep();
                Console.WriteLine($"#{index}: {signature}");
                Interlocked.Increment(ref ChunksHandled);
            }
        }
    }
}
