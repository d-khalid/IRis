using System;
using System.Collections.Generic;
using IRis.Services.Commands;
using IRis.Services.Singleton;

namespace IRis.Services;


public static class CommandService
{
    private static readonly Stack<CommandBase> _undoStack = new();
    private static readonly Stack<CommandBase> _redoStack = new();


    public static void Execute(CommandBase command)
    {
        command.Execute();
        _undoStack.Push(command);

        AppState.Get().LastCommand = command.Name;
    }


    public static void Undo()
    {
        if (_undoStack.Count == 0 || !AppState.Get().EditingAllowed) return;

        var command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);

        AppState.Get().LastCommand = "Undo: " + command.Name;
    }


    public static void Redo()
    {
        if (_redoStack.Count == 0 || !AppState.Get().EditingAllowed) return;

        var command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);

        AppState.Get().LastCommand = "Redo: " + command.Name;
    }


    public static void Reset()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        AppState.Get().LastCommand = "(no action yet)";
    }
}
