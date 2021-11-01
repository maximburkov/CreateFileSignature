using System;
using System.Threading;

namespace CreateFileSignature.WorkerPool
{
    /// <summary>
    /// Worker wrapper for thread.
    /// </summary>
    public class Worker
    {
        private Thread thread;
        private Action action;

        public Worker()
        {
        }

        public Worker(Action action) : base()
        {
            this.AssignAction(action);
        }

        /// <summary>
        /// Indicates if we have work assigned for this worker.
        /// </summary>
        public bool HasWork { get; private set; }

        /// <summary>
        /// Indicates if stop was requested and we shouldn't expect new action assigned.
        /// </summary>
        public bool IsStopRequested { get; private set; }

        /// <summary>
        /// Index of worker. 
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Starts worker processing in new thread.
        /// </summary>
        public void Start()
        {
            thread = new Thread(Process);
            thread.Start();
        }

        /// <summary>
        /// Assign action for processing.
        /// </summary>
        /// <param name="action"></param>
        public void AssignAction(Action action)
        {
            this.action = action;
            HasWork = true;
        }

        /// <summary>
        /// Stops processing loop and waiting for finishsing of assigned job.
        /// </summary>
        public void Stop()
        {
            this.IsStopRequested = true;
            thread.Join();
        }

        /// <summary>
        /// Checks if worker has action assigned until stop is requested.
        /// </summary>
        private void Process()
        {
            while (!this.IsStopRequested)
            {
                if (this.HasWork)
                {
                    action?.Invoke();
                    action = null;
                    this.HasWork = false;
                }
            }
        }
    }
}
