namespace CreateFileSignature.Output
{
    public interface IOutput
    {
        void Write(int index, string message);

        void Summarize();

        int LinesHandled { get; }
    }
}
