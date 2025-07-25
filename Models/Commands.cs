using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using IRis.Models.Core;

namespace IRis.Models.Commands
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    public class AddComponentCommand : ICommand
    {
        private readonly Canvas _canvas;
        private readonly List<Component> _components;
        private readonly Component _component;
        private readonly Point _position;

        public AddComponentCommand(Canvas canvas, List<Component> components, Component component, Point position)
        {
            _canvas = canvas;
            _components = components;
            _component = component;
            _position = position;
        }

        public void Execute()
        {
            Canvas.SetLeft(_component, _position.X);
            Canvas.SetTop(_component, _position.Y);
            _canvas.Children.Add(_component);
            _components.Add(_component);
        }

        public void Undo()
        {
            _canvas.Children.Remove(_component);
            _components.Remove(_component);
        }
    }

    public class DeleteComponentsCommand : ICommand
    {
        private readonly Canvas _canvas;
        private readonly List<Component> _components;
        private readonly List<Component> _deletedComponents;
        private readonly List<Point> _positions;

        public DeleteComponentsCommand(Canvas canvas, List<Component> components, List<Component> toDelete)
        {
            _canvas = canvas;
            _components = components;
            _deletedComponents = new List<Component>(toDelete);
            _positions = toDelete.Select(c => new Point(Canvas.GetLeft(c), Canvas.GetTop(c))).ToList();
        }

        public void Execute()
        {
            foreach (var component in _deletedComponents)
            {
                _canvas.Children.Remove(component);
                _components.Remove(component);
            }
        }

        public void Undo()
        {
            for (int i = 0; i < _deletedComponents.Count; i++)
            {
                var component = _deletedComponents[i];
                Canvas.SetLeft(component, _positions[i].X);
                Canvas.SetTop(component, _positions[i].Y);
                _canvas.Children.Add(component);
                _components.Add(component);
            }
        }
    }

    public class MoveComponentsCommand : ICommand
    {
        private readonly List<Component> _components;
        private readonly List<Point> _oldPositions;
        private readonly List<Point> _newPositions;

        public MoveComponentsCommand(List<Component> components, List<Point> oldPositions, List<Point> newPositions)
        {
            _components = new List<Component>(components);
            _oldPositions = new List<Point>(oldPositions);
            _newPositions = new List<Point>(newPositions);
        }

        public void Execute()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                Canvas.SetLeft(_components[i], _newPositions[i].X);
                Canvas.SetTop(_components[i], _newPositions[i].Y);
                _components[i].InvalidateVisual();
            }
        }

        public void Undo()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                Canvas.SetLeft(_components[i], _oldPositions[i].X);
                Canvas.SetTop(_components[i], _oldPositions[i].Y);
                _components[i].InvalidateVisual();
            }
        }
    }

    public class CommandManager
    {
        private readonly Stack<ICommand> _undoStack = new();
        private readonly Stack<ICommand> _redoStack = new();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear(); // Clear redo stack when new command is executed
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}