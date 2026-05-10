using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using IRis.Models.Components;
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
        private readonly List<Point> _originalPositions;

        public DeleteComponentsCommand(Canvas canvas, List<Component> components, List<Component> selectedComponents)
        {
            _canvas = canvas;
            _components = components;
            _deletedComponents = new List<Component>(selectedComponents);
            _originalPositions = selectedComponents.Select(c => 
                new Point(Canvas.GetLeft(c), Canvas.GetTop(c))).ToList();
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
                var position = _originalPositions[i];
                
                Canvas.SetLeft(component, position.X);
                Canvas.SetTop(component, position.Y);
                _canvas.Children.Add(component);
                _components.Add(component);
            }
        }
    }

    public class MoveComponentsCommand : ICommand
    {
        private readonly List<Component> _components;
        private readonly List<Point> _originalPositions;
        private readonly List<Point> _newPositions;
        private readonly Canvas _canvas;

        // Constructor with 2 arguments (components and offset)
        public MoveComponentsCommand(List<Component> components, Point offset)
        {
            _components = new List<Component>(components);
            _canvas = null!;
            
            // Calculate original and new positions based on offset
            _originalPositions = _components.Select(c => 
                new Point(Canvas.GetLeft(c), Canvas.GetTop(c))).ToList();
            _newPositions = _originalPositions.Select(pos => 
                new Point(pos.X + offset.X, pos.Y + offset.Y)).ToList();
        }

        // Constructor with 3 arguments (canvas, components, newPositions) - for SelectionManager compatibility
        public MoveComponentsCommand(Canvas canvas, List<Component> components, List<Point> newPositions)
        {
            _canvas = canvas;
            _components = new List<Component>(components);
            _newPositions = new List<Point>(newPositions);
            
            // Store original positions for undo
            _originalPositions = _components.Select(c => 
                new Point(Canvas.GetLeft(c), Canvas.GetTop(c))).ToList();
        }

        public void Execute()
        {
            for (int i = 0; i < _components.Count && i < _newPositions.Count; i++)
            {
                Canvas.SetLeft(_components[i], _newPositions[i].X);
                Canvas.SetTop(_components[i], _newPositions[i].Y);
                _components[i].InvalidateVisual();
            }
        }

        public void Undo()
        {
            for (int i = 0; i < _components.Count && i < _originalPositions.Count; i++)
            {
                Canvas.SetLeft(_components[i], _originalPositions[i].X);
                Canvas.SetTop(_components[i], _originalPositions[i].Y);
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
                Console.WriteLine($"Undoing: {command.GetType().Name}");
                command.Undo();
                _redoStack.Push(command);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                Console.WriteLine($"Redoing: {command.GetType().Name}");
                command.Execute();
                _undoStack.Push(command);
            }
        }
    }
}