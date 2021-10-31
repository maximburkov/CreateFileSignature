using System.Threading;

namespace CreateFileSignature.Output
{
    public abstract class ConsoleOutput : IOutput
    {
        protected int linesHandled;

        public int LinesHandled => this.linesHandled;

        public abstract void Summarize();

        public virtual void Write(int index, string message)
        {
            Interlocked.Increment(ref this.linesHandled);
        }
    }
}
