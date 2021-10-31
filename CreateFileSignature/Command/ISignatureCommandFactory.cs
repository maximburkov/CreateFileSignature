namespace CreateFileSignature.Command
{
    interface ISignatureCommandFactory
    {
        public ICommand Create(int index, byte [] bytes);
    }
}
