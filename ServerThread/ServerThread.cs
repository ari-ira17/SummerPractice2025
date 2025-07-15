using Command;
using System;
using System.Collections.Concurrent;
using System.Threading;

public class HardStopCommand : ICommand
{
    private ServerThread _thread;

    public HardStopCommand(ServerThread thread)
    {
        _thread = thread;
    }

    public void Execute()
    {
        if (Thread.CurrentThread.ManagedThreadId == _thread.id)
        {
            _thread.HardStop();
        }
        else
        {
            throw new ("HardStop может быть выполнена только в том потоке, который она останавливает.");
        }
    }
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
}

public class ServerThread
{
    private ConcurrentQueue<ICommand> commands = new ConcurrentQueue<ICommand>();
    private bool hard_stop = false;
    private bool soft_stop = false;
    public bool is_running = true;

    public int id { get; set; }

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
        while(!hard_stop)
        {
            if(commands.TryDequeue(out ICommand command))
            {
                command.Execute();                
            }
            else
            {
                if(soft_stop)
                {
                    is_running = false; 
                    break;
                }
                else
                {
                    Thread.Sleep(100); 
                    
                    continue;
                }
            }
        }

        is_running = false;
    }
}
