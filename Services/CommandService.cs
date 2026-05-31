using System.Collections.Generic;
using IRis.Services.Commands;
using IRis.Services.Singleton;

namespace IRis.Services;


public static class CommandService
{
    private static readonly Stack<ICommand> _undoStack = new();
    private static readonly Stack<ICommand> _redoStack = new();


    public static void Execute(ICommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        AppState.Get().LastCommand = command.ToString()!.Split(".")[^1];
    }


    public static void Undo()
    {
        if (_undoStack.Count == 0 || Simulation.Get().Running) return;

        var command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);
    }


    public static void Redo()
    {
        if (_redoStack.Count == 0 || Simulation.Get().Running) return;

        var command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);
    }


    public static void Reset()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        AppState.Get().LastCommand = "none";
    }
}
