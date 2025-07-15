using Command;
using System.Threading;

public class ServerThreadTests
{
    [Fact(Timeout = 1000)] 
    public void SoftStopCommand_Should_Allow_AllCommands_ToComplete_BeforeStopping()
    {
        var server_thread = new ServerThread();
        server_thread.Start();

        for (int i = 0; i < 10; i++)
        {
            server_thread.AddToQueue(new BasicCommand());
        }

        server_thread.AddToQueue(new SoftStopCommand(server_thread));

        while (server_thread.is_running)
        {
            Thread.Sleep(50);
        }

        Assert.False(server_thread.is_running);
    }

    [Fact(Timeout = 1000)]
    public void HardStopCommand_Should_StopThread_Immediately()
    {
        var server_thread = new ServerThread();
        server_thread.Start();

        for (int i = 0; i < 10; i++)
        {
            server_thread.AddToQueue(new BasicCommand());
        }

        server_thread.AddToQueue(new HardStopCommand(server_thread));

        while (server_thread.is_running)
        {
            Thread.Sleep(50);
        }

        Assert.False(server_thread.is_running);
    }

    [Fact]
    public void HardStopCommand_Should_Throw_When_ExecutedFromWrongThread()
    {
        var server_thread = new ServerThread();
        server_thread.Start();

        var hard_stop = new HardStopCommand(server_thread);

        var expected = Assert.Throws<System.Exception>(() => hard_stop.Execute());
        Assert.Contains("HardStop может быть выполнена только в том потоке", expected.Message);
    }

    [Fact]
    public void SoftStopCommand_Should_Throw_When_ExecutedFromWrongThread()
    {
        var server_thread = new ServerThread();
        server_thread.Start();

        var soft_stop = new SoftStopCommand(server_thread);

        var expected = Assert.Throws<System.Exception>(() => soft_stop.Execute());
        Assert.Contains("SoftStop  быть выполнена только в том потоке", expected.Message);
    }
}

public class BasicCommand : ICommand
{
    public void Execute()
    {
        int result = 1;

        for (int i = 1; i < 5000; i++)
        {
            result *= i;
        }
    }
}
