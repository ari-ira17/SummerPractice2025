using Command;
using System;
using System.Collections.Concurrent;
using System.Threading;

public class ServerThread
{
    private ConcurrentQueue<ICommand> commands = new ConcurrentQueue<ICommand>();
    private bool hard_stop = false;
    private bool soft_stop = false;
    public bool is_running = true;
    private IScheduler scheduler = new RoundRobinScheduler();
    public int id { get; set; }
    private bool use_scheduler = true;

    public void Start()
    {
        Thread thread = new Thread(Run);
        thread.Start();
        id = thread.ManagedThreadId;
    }

    public void AddToQueue(ICommand command)
    {    
        if (command is HardStopCommand)
        {
            commands.Clear();
            commands.Enqueue(command);
        }        
        else {commands.Enqueue(command);}
    }

    public void HardStop()
    {
        hard_stop = true;
    }

    public void SoftStop()
    {
        soft_stop = true;
    }

    private void Run()
    {
        while (!hard_stop)
        {
            if (scheduler.HasCommand() && (use_scheduler || !(commands.Count() > 0)))
            {
                var command = scheduler.Select();
                if (command != null)
                {
                    command.Execute();
                    if (!command.IsCompleted())
                    {
                        scheduler.Add(command);
                    }
                }
                use_scheduler = false;
            }
            else if (commands.TryDequeue(out ICommand command))
            {
                command.Execute();
                if (!command.IsCompleted())
                {
                    scheduler.Add(command);
                }
                use_scheduler = true;
            }
            else
            {
                if (soft_stop) { break; }
                Thread.Sleep(100);
                use_scheduler = true;
            }
        }

        is_running = false;
    }
}

public class HardStopCommand : ICommand
{
    private ServerThread _thread;

    public HardStopCommand(ServerThread thread)
    {
        _thread = thread;
    }

    public void Execute()
    {
        if(Thread.CurrentThread.ManagedThreadId == _thread.id)
        {
            _thread.HardStop();
        }
        else
        {
            throw new ("HardStop может быть выполнена только в том потоке, который она останавливает.");
        }
    }
    public bool IsCompleted() { return true; } 
}

public class SoftStopCommand : ICommand
{
    private ServerThread _thread;

    public SoftStopCommand(ServerThread thread)
    {
        _thread = thread;
    }

    public void Execute()
    {
        if(Thread.CurrentThread.ManagedThreadId == _thread.id) 
        {
            _thread.SoftStop();
        }
        else
        {
            throw new ("SoftStop  быть выполнена только в том потоке, который она останавливает.");
        }
    }
    public bool IsCompleted() { return true; } 
}
