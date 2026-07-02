using System.Collections.Generic;
using IRis.Services.Commands;
using IRis.Services.Singleton;
using Microsoft.Extensions.DependencyInjection;

namespace IRis.Services;

public static class CommandService
{
    private static readonly Stack<CommandBase> _undoStack = new();
    private static readonly Stack<CommandBase> _redoStack = new();
    private static readonly AppState _appState =
        App.Current.Services.GetRequiredService<AppState>();

    public static void Execute(CommandBase command)
    {
        command.Execute();
        _undoStack.Push(command);

        _appState.LastCommand = command.Name;
    }

    public static void Undo()
    {
        if (_undoStack.Count == 0 || !_appState.EditingAllowed)
            return;

        var command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);

        _appState.LastCommand = "Undo: " + command.Name;
    }

    public static void Redo()
    {
        if (_redoStack.Count == 0 || !_appState.EditingAllowed)
            return;

        var command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);

        _appState.LastCommand = "Redo: " + command.Name;
    }

    public static void Reset()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        _appState.LastCommand = "(no action yet)";
    }
}
