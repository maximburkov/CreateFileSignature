using System;
using System.Collections.Concurrent;
using System.Linq;

namespace CreateFileSignature.Output
{
    class OrderedConsoleOutput : ConsoleOutput
    {
        private ConcurrentDictionary<int, string> outputs;

        public OrderedConsoleOutput()
        {
            outputs = new ConcurrentDictionary<int, string>();
        }

        public override void Write(int index, string message)
        {
            base.Write(index, message);
            outputs[index] = message;
        }

        public override void Summarize()
        {
            foreach (var item in outputs.OrderBy(i => i.Key).ToList())
            {
                Console.WriteLine($"#{item.Key}: {item.Value}");
            }

            Console.WriteLine($"Number of handled chunks: {this.linesHandled}");
        }
    }
}
