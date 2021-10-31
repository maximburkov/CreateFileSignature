using System;

namespace CreateFileSignature.Output
{
    public class RealTimeConsoleOutput : ConsoleOutput
    {
        public override void Write(int index, string message)
        {
            base.Write(index, message);
            Console.WriteLine($"#{index}: {message}");
        }

        public override void Summarize()
        {
            Console.WriteLine($"Number of handled chunks: {this.linesHandled}");
        }
    }
}
