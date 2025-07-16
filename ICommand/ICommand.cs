namespace Command;

public interface ICommand
{
    void Execute();
    bool IsCompleted();
}

public class TestCommand(int id) : ICommand
{
    int counter = 0;

    public void Execute()
    {
        Console.WriteLine($"Поток {id} вызов {++counter}");
    }

    public bool IsCompleted()
    {
        return counter >= 2;
    }
}
