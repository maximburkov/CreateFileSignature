using System;
using System.Security.Cryptography;

namespace CreateFileSignature
{
    /// <summary>
    /// TODO
    /// </summary>
    public class PrintSignatureCommand : ICommand
    {
        private int index;
        private byte[] bytes;


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
            using (SHA256 sha256 = SHA256.Create())
            {
                var signature = BitConverter.ToString(sha256.ComputeHash(bytes));
                //var thread = Thread.CurrentThread.ManagedThreadId;
                //Console.WriteLine($"#{Index}, t: {thread}: {res}");
                Console.WriteLine($"#{index}: {signature}");
            }
        }
    }
}
