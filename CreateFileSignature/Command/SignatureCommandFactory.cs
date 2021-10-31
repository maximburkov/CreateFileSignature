using CreateFileSignature.Output;
using System;

namespace CreateFileSignature.Command
{
    public class SignatureCommandFactory : ISignatureCommandFactory
    {
        private IOutput output;

        public SignatureCommandFactory(IOutput output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public ICommand Create(int index, byte[] bytes)
        {
            return new SignatureCommand(index, bytes, output);
        }
    }
}
