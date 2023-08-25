namespace GameServer.Job
{
    public class JobQueue
    {
        object queueLock = new object();
        Queue<IJob> queue = new Queue<IJob>();

        public void Push(IJob job)
        {
            lock (queueLock)
            {
                queue.Enqueue(job);
            }
        }

        public void Flush()
        {
            while (true)
            {
                IJob? job = Pop();
                if (job == null)
                {
                    return;
                }
                else
                {
                    job.Execute();
                }
            }
        }

        IJob? Pop()
        {
            lock (queueLock)
            {
                if (queue.Count == 0)
                {
                    return null;
                }
                else
                {
                    return queue.Dequeue();
                }
            }
        }

        public void Push(Action action)
        {
            Push(new Job(action));
        }
        public void Push<T1>(Action<T1> action, T1 t1)
        {
            Push(new Job<T1>(action, t1));
        }
        public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
        {
            Push(new Job<T1, T2>(action, t1, t2));
        }
        public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            Push(new Job<T1, T2, T3>(action, t1, t2, t3));
        }
        public void Push<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            Push(new Job<T1, T2, T3, T4>(action, t1, t2, t3, t4));
        }
    }
}
