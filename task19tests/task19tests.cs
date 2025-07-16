using Command;
using System.Threading;

public class ServerThreadTests
{
    [Fact]
    public void Main_Test()
    {
        var server_thread = new ServerThread();
        var test_commands = new TestCommand[]
        {
            new TestCommand(1),
            new TestCommand(2),
            new TestCommand(3),
            new TestCommand(4),
            new TestCommand(5)
        };

        server_thread.Start();

        for (int i = 0; i < test_commands.Length; i++)
        {
            server_thread.AddToQueue(test_commands[i]);
        }

        while (server_thread.is_running)
        {
            bool check = true;
            foreach (TestCommand test_command in test_commands)
            {
                check = check && test_command.IsCompleted();
            }

            if (check)
            {
                server_thread.AddToQueue(new HardStopCommand(server_thread));
                break;
            }

            Thread.Sleep(100);
        }

        Thread.Sleep(500);

        Assert.True(test_commands.All(command => command.IsCompleted()));
        Assert.False(server_thread.is_running);
    }
}
