using CreateFileSignature.Command;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CreateFileSignature.WorkerPool
{
    /// <summary>
    /// Pool for Managed workers.
    /// </summary>
    public class WorkerPool : IDisposable
    {
        private Thread queueThread;
        private List<Worker> workers;
        private int maxThreadCount;
        private ConcurrentQueue<ICommand> comandQueue = new ConcurrentQueue<ICommand>();
        private bool isDisposed = false;

        public WorkerPool(int threadCount)
        {
            workers = new List<Worker>(threadCount);
            this.maxThreadCount = threadCount;
            this.queueThread = new Thread(Process);
            queueThread.Start();
        }

        /// <summary>
        /// Enqueue command to queue.
        /// </summary>
        /// <param name="command">Command.</param>
        public void Enqueue(ICommand command)
        {
            comandQueue.Enqueue(command);
        }

        /// <summary>
        /// Tries to assing worker for command. If there is no available workers tries to create new one.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <returns>True if we added new worker or assigned command to existing, otherwise false.</returns>
        private bool TryAssignWorker(ICommand command)
        {
            Worker worker = workers.FirstOrDefault(w => !w.HasWork);

            if (worker is null)
            {
                if (workers.Count < maxThreadCount)
                {
                    worker = new Worker(command.Execute);
                    workers.Add(worker);
                    worker.Start();
                    return true;
                }
            }
            else
            {
                worker.AssignAction(command.Execute);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Processign queue while stop is not requested.
        /// </summary>
        private void Process()
        {
            while (true)
            {
                if (comandQueue.TryDequeue(out var command))
                {
                    bool commandQueued = false;
                    while (!commandQueued)
                    {
                        commandQueued = TryAssignWorker(command);
                    }
                }
                else
                {
                    if (isDisposed)
                    {
                        foreach (var worker in workers)
                        {
                            worker.Stop();
                        }

                        return;
                    }
                }
            }
        }

        public void Dispose()
        {
            isDisposed = true;
            queueThread.Join();
        }
    }
}
