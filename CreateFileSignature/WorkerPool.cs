﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace CreateFileSignature
{
    public class IndexedAction
    {
        public int Index { get; set; }

        public byte[] Bytes { get; set; }

        public void Execute()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var res = BitConverter.ToString(sha256.ComputeHash(Bytes));
                //var res = HashToString(sha256.ComputeHash(Bytes));

                var thread = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine($"#{Index}, t: {thread}: {res}");
            }
        }

        private string HashToString(byte [] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }

    public class WorkerPool : IDisposable
    {
        private List<Thread> threads;
        int createdThreadsCount = 0;
        int freeThreadsCount = 0;
        private Thread queueThread;
        private List<Worker> workers;

        private int threadCount;

        private ConcurrentQueue<IndexedAction> queue = new ConcurrentQueue<IndexedAction>(); // TODO: check if we have issues with action
        private bool isDisposed = false;

        public WorkerPool(int threadCount)
        {
            //threads = new List<Thread>(threadCount);
            workers = new List<Worker>(threadCount);
            this.threadCount = threadCount;
            this.queueThread = new Thread(Process);
            queueThread.Start();
        }

        public int QueueLength => queue.Count;

        public void Dispose()
        {
            isDisposed = true;

            //for (int i = 0; i < threads.Count; i++)
            //{
            //    threads[i].Join();
            //}

            // TODO: check if I need wait both of them. 
        }

        public void Finsihwork()
        {
            isDisposed = true;

            queueThread.Join();
        }

        private void WaitWorkers()
        {
            foreach (var worker in workers)
            {
                worker.Stop();
                worker.Wait();
            }
        }

        public void Enqueue(IndexedAction action)
        {
            Console.WriteLine($"Enqueued action {action.Index}");
            queue.Enqueue(action);
        }

        private void TryToStartTask()
        {
            if (freeThreadsCount <= 0)
            {
                var thread = new Thread(new ThreadStart(Process));
                threads.Add(thread);
                thread.Start();
                Interlocked.Increment(ref createdThreadsCount);
                Interlocked.Increment(ref freeThreadsCount);
                //Console.WriteLine($"Another thread started...{freeThreadsCount}");
            }
        }

        private Worker GetWorker()
        {
            for (int i = 0; i < workers.Count; i++)
            {
                if(workers[i].Status == Status.Free)
                {
                    return workers[i];
                }
            }

            return null;
        }

        private void Process()
        {
            while (true)
            {
                if (queue.TryDequeue(out var indexedAction))
                {
                    Console.WriteLine($"Dequeued action {indexedAction.Index}");

                    bool taskQueued = false;
                    while (!taskQueued)
                    {
                        var worker = GetWorker();

                        if (worker is null)
                        {
                            if (workers.Count < threadCount)
                            {
                                worker = new Worker(indexedAction.Execute);
                                workers.Add(worker);
                                worker.Index = workers.Count;

                                Console.WriteLine($"Another wroker thread started...{indexedAction.Index}");
                                worker.Start();
                                taskQueued = true;
                            }
                            else
                            {
                                //Console.WriteLine("Do not create thread. Max threads");
                            }
                        }
                        else
                        {
                            worker.AssignAction(indexedAction.Execute);
                            taskQueued = true;
                        }
                    }
                }
                else
                {
                    if (isDisposed)
                    {
                        WaitWorkers();
                        return;
                    }
                }
            }
        }
    }
}