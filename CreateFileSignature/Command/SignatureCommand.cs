using CreateFileSignature.Output;
using System;
using System.Security.Cryptography;

namespace CreateFileSignature.Command
{
    /// <summary>
    /// Get file chunk signature command.
    /// </summary>
    public class SignatureCommand : ICommand
    {
        private int index;
        private byte[] bytes;
        private IOutput output;

        public int Index => index;

        public SignatureCommand(int index, byte[] bytes, IOutput output)
        {
            this.index = index;
            this.bytes = bytes;
            this.output = output;
        }

        /// <summary>
        /// Executes 
        /// </summary>
        public void Execute()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var signature = BitConverter.ToString(sha256.ComputeHash(bytes));
                output.Write(index, signature);
            }
        }
    }
}
