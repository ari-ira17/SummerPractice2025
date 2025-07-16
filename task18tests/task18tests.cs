using Command;
using System.Threading;

public class SchedulerTests
{
        [Fact]
    public void TestRoundRobinScheduling()
    {
        var server_thread = new ServerThread();
        server_thread.Start();

        var long_running_command = new LongRunningCommand();
        var basic_command = new BasicCommand();
        server_thread.AddToQueue(long_running_command);
        server_thread.AddToQueue(basic_command);
        
        Thread.Sleep(10); 

        Assert.False(long_running_command.IsCompleted());
        Assert.True(basic_command.IsCompleted());
    }

    [Fact]
    public void TestLongRunningCommand()
    {
        var server_thread = new ServerThread();
        server_thread.Start();

        var long_running_command = new LongRunningCommand();
        server_thread.AddToQueue(long_running_command);

        Thread.Sleep(5);

        Assert.False(long_running_command.IsCompleted());

        Thread.Sleep(1000);
        
        Assert.True(long_running_command.IsCompleted());
    }
}

public class BasicCommand : ICommand
{
    public void Execute()
    {
        int result = 1;

        for (int i = 1; i < 10000; i++)
        {
            result *= i;
        }
    }
    public bool IsCompleted() { return true; }
}

public class LongRunningCommand : ICommand
{
    private int steps = 1000;
    private int current_step = 0;

    public void Execute()
    {
        if (current_step < steps)
        {
            current_step++;
        }
    }

    public bool IsCompleted() { return current_step >= steps; } 
}
