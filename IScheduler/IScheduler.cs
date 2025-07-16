using Command;

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand command);
}