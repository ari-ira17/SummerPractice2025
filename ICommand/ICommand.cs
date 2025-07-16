namespace Command;

public interface ICommand
{
    void Execute();
    bool IsCompleted();
}
