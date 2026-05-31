namespace IRis.Services.Commands;


public abstract class CommandBase
{
    public string Name = "(unknown action)";
    public abstract void Execute();
    public abstract void Undo();
}
