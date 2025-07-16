using Command;
using System;
using System.Collections.Concurrent;
using System.Threading;

public class RoundRobinScheduler : IScheduler
{
    private ConcurrentQueue<ICommand> commandQueue = new ConcurrentQueue<ICommand>();
    private readonly object lockObject = new object();

    public bool HasCommand()
    {
        return commandQueue.Count() > 0;
    }

    public ICommand Select()
    {
        lock (lockObject)
        {
            if (commandQueue.TryDequeue(out ICommand cmd))
            {
                return cmd;
            }
            return null;
        }
    }

    public void Add(ICommand cmd)
    {
        commandQueue.Enqueue(cmd);
    }
}
