using Command;
using System;
using System.Collections.Concurrent;
using System.Threading;

public class RoundRobinScheduler : IScheduler
{
    private ConcurrentQueue<ICommand> command_queue = new ConcurrentQueue<ICommand>();
    private readonly object lock_object = new object();

    public bool HasCommand()
    {
        return command_queue.Count() > 0;
    }

    public ICommand Select()
    {
        lock (lock_object)
        {
            if (command_queue.TryDequeue(out ICommand command))
            {
                return command;
            }
            return null;
        }
    }

    public void Add(ICommand command)
    {
        command_queue.Enqueue(command);
    }
}
