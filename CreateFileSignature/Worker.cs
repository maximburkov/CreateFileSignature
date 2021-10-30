using System;
using System.Threading;

namespace CreateFileSignature
{
    public enum Status 
    { 
        Free, 
        Busy,
        Stopped
    }

    public class Worker
    {
        private Thread thread;
        private bool hasWork;
        private bool inProgress;

        public Worker()
        {
        }

        public Worker(Action action) : base()
        {
            this.AssignAction(action);
        }

        public void Start()
        {
            thread = new Thread(Process);
            thread.Start();
        }

        public void AssignAction(Action action)
        {
            this.Action = action;
            hasWork = true;
        }

        public void Wait()
        {
            Console.WriteLine($"Waiting worker {Index}");
            thread.Join();
        }

        public void Stop()
        {
            Console.WriteLine($"Stopping worker {Index}");
            Status = Status.Stopped;
            inProgress = false;
        }

        public int Index { get; set; }

        public Status Status { get; set; } = Status.Free;

        public Action Action { get; set; }

        public void Process()
        {
            inProgress = true;
            while (inProgress)
            {
                if(hasWork && this.Status == Status.Free)
                {
                    this.Status = Status.Busy;
                    Action?.Invoke();
                    hasWork = false;
                    this.Status = Status.Free;
                }
            }
        }
    }
}
