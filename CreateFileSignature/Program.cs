using CreateFileSignature.Command;
using CreateFileSignature.Output;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CreateFileSignature
{
    class Program
    {
        static void PrintHelp()
        {
            string helpText = 
                @$"Welcome to 'Create signature' program.
This program generates SHA-256 signature of file chunks.
You can run this program with command line parameters or set them during execution.
Parameters list:
1: Path to the file.
2. Chunk size (bytes by default). You can specify measure after number (without spaces): b - bytes, k - killobytes, m - megabytes. 
Shouldn't be bigger than {int.MaxValue} bytes, {int.MaxValue / 1024} kb, {int.MaxValue / 1024 / 1024} mb.
3. Should we order signature results according chunks index: y/n (n by default):

Command example: 'CreateFileSignature Test.txt 100m n' - this command will parse Text.txt file to chunks of size 100 mb and will print it in unordred fasion.";

            Console.WriteLine(helpText);
        }

        static bool TryGetChunkSizeFromParameter(string parameter, out int sizeInBytes)
        {
            try
            {
                checked
                {
                    var lastSymmbol = parameter[^1].ToString().ToUpperInvariant();

                    int multiplier = lastSymmbol switch
                    {
                        "K" => 1024,
                        "M" => 1024 * 1024,
                        _ => 1
                    };

                    string stringToParse = new[] { "B", "K", "M" }.Contains(lastSymmbol) ? parameter[..^1] : parameter;

                    if (!int.TryParse(stringToParse, out int result))
                    {
                        throw new Exception("Block size format is incorrect.");
                    }

                    sizeInBytes = multiplier * result;
                    return true;
                }
            }
            catch(OverflowException ex)
            {
                throw new Exception($"Block size is to big. Shouldn't be bigger than {int.MaxValue} bytes, {int.MaxValue/1024} kb, {int.MaxValue / 1024 / 1024} mb.");
            }
            catch (Exception)
            {
                sizeInBytes = -1;
                return false;
            }          
        }

        static void Main(string[] args)
        {
            int chunkLength = 1024 * 1024 * 100; // default chunk size 100 mb
            string filePath = string.Empty;          
            string chunkSizeParam = chunkLength.ToString();
            const string isOrdreredYes = "Y", isOrderedNo = "N";
            string isOrderedParam = isOrderedNo;

            try
            {
                if (args.Length == 0)
                {
                    PrintHelp();
                    Console.Write("Path to file:");
                    filePath = Console.ReadLine();
                    Console.Write("Chunk size and measure (b/k/m) without spaces:");
                    chunkSizeParam = Console.ReadLine();
                    Console.Write("Should order output (y/n). Using 'y' parameter can affect performance:");
                    isOrderedParam = Console.ReadLine();
                }

                if (args.Length >= 1)
                    filePath = args[0];

                if (args.Length >= 2)
                {
                    chunkSizeParam = args[1];
                }

                if (args.Length >= 3)
                    isOrderedParam = args[2];

                if (!File.Exists(filePath))
                    throw new Exception("File does not exists.");

                if(!TryGetChunkSizeFromParameter(chunkSizeParam, out chunkLength))
                {
                    throw new Exception("Block size format is incorrect.");
                }

                isOrderedParam = isOrderedParam.ToUpperInvariant();
                if (isOrderedParam != isOrdreredYes && isOrderedParam != isOrderedNo)
                {
                    throw new Exception("Order parameter is incorrect. Should be 'Y' or 'N'.");
                }

                IOutput output = isOrderedParam == isOrdreredYes ? new OrderedConsoleOutput() : new RealTimeConsoleOutput();
                ISignatureCommandFactory commandFactory = new SignatureCommandFactory(output);

                int threadCount = Environment.ProcessorCount;
                int chunkIndex = 0;
                int bytesReaded;

                Stopwatch sw = new Stopwatch();
                sw.Start();

                using (var pool = new WorkerPool.WorkerPool(threadCount))
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    do
                    {
                        byte[] chunk = new byte[chunkLength];
                        bytesReaded = fileStream.Read(chunk, 0, chunkLength);
                        var command = commandFactory.Create(++chunkIndex, chunk);
                        pool.Enqueue(command);
                    }
                    while (bytesReaded >= chunkLength);
                }

                output.Summarize();
                sw.Stop();
                Console.WriteLine($"Time: {sw.Elapsed.TotalMilliseconds}ms");
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(ex.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}