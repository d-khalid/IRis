using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using Avalonia;
using System.Collections.Generic;

using IRis.Models.Core;
using IRis.Models.Components;
using System.Linq;


namespace IRis.Models;


public partial class Simulation
{   
    private bool _isSelecting = false;
    private Point _selectStartPosition;
    private Avalonia.Controls.Shapes.Rectangle? _selectionRectangle = null;


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            foreach (Component c in Components)
            {
                if (c is LogicToggle lt && lt.HitTest(CurrentMousePos))
                {
                    lt.Toggle();
                    return;
                }
            }
        }

        else if (PreviewObject != null)
        {
            if (PreviewObject is Component c)
            {
                c.IsPreview = false;
                Components.Add(c);

                // create a new one instantly
                PreviewObject = (CircuitObject)PreviewObject.Clone();
                _canvas.Children.Add(PreviewObject);

                Canvas.SetLeft(PreviewObject, CurrentMousePos.X);
                Canvas.SetTop(PreviewObject, CurrentMousePos.Y);

                if (PreviewObject is Component comp)
                {
                    comp.Position = CurrentMousePos;
                }
            }

            else if (PreviewObject is Wire wire)
            {
                Terminal? t = null;
                
                foreach (Component comp in Components)
                {
                    t = comp.GetTerminalHitTest(CurrentMousePos);
                    if (t != null) break;
                }

                if (t != null)
                {
                    wire.RemoveLastNode();
                    wire.AddNode(
                        terminal: t,
                        position: CurrentMousePos,
                        isOutputProvider: t.IsOutputProvider
                    );

                    if (wire.Nodes.Count == 2) 
                    {
                        wire.IsPreview = false;
                        Wires.Add(wire);

                        // create a new one instantly
                        PreviewObject = (CircuitObject)PreviewObject.Clone();
                        _canvas.Children.Add(PreviewObject);

                        if (PreviewObject is Wire w)
                        { 
                            w.AddNode(
                                terminal: new Terminal(CurrentMousePos),
                                position: CurrentMousePos,
                                isOutputProvider: false
                            );
                        }
                    }

                    else
                    {
                        wire.AddNode(
                            terminal: new Terminal(CurrentMousePos),
                            position: CurrentMousePos,
                            isOutputProvider: false
                        );
                    }
                }
            }
        }

        else if (!IsSimulating)     // selection logic
        {
            bool hasHitComponent = false;
            foreach (Component c in Components)
            {
                if (c.HitTest(CurrentMousePos) && !c.IsSelected)
                {
                    c.IsSelected = true;
                    hasHitComponent = true;
                }

                else if (c.IsSelected)
                {
                    c.IsSelected = false;
                }
            }

            foreach (Wire wire in Wires)
            {
                if (wire.HitTest(CurrentMousePos) && !wire.IsSelected)
                {
                    wire.IsSelected = true;
                }

                else if (wire.IsSelected)
                {
                    wire.IsSelected = false;
                }
            }

            if (!hasHitComponent)
            {
                _isSelecting = true;
                _selectStartPosition = CurrentMousePos;

                _selectionRectangle = new Avalonia.Controls.Shapes.Rectangle
                {
                    Fill = new SolidColorBrush(Colors.DodgerBlue, 0.2),
                    Stroke = new SolidColorBrush(Colors.DodgerBlue, 0.6),
                    StrokeThickness = 2
                };

                _canvas.Children.Add(_selectionRectangle);
            }
        }
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isSelecting && _selectionRectangle != null)
        {
            _isSelecting = false;
            _canvas.Children.Remove(_selectionRectangle);
        }
    }
    

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        CurrentMousePos = Utils.SnapPointToGrid(e.GetPosition(_canvas));
        Point currPos = e.GetPosition(_canvas);


        if (_isSelecting && _selectionRectangle != null)
        {
            Canvas.SetLeft(
                _selectionRectangle, 
                Math.Min(currPos.X, _selectStartPosition.X)
            );

            Canvas.SetTop(
                _selectionRectangle, 
                Math.Min(currPos.Y, _selectStartPosition.Y)
            );

            _selectionRectangle.Width = Math.Abs(currPos.X - _selectStartPosition.X);
            _selectionRectangle.Height = Math.Abs(currPos.Y - _selectStartPosition.Y);


            // select all within range
            Rect range = new(
                x: Canvas.GetLeft(_selectionRectangle),
                y: Canvas.GetTop(_selectionRectangle),
                width: _selectionRectangle.Width,
                height: _selectionRectangle.Height
            );

            foreach (Component c in Components)
            {
                Rect target = new(
                    x: Canvas.GetLeft(c),
                    y: Canvas.GetTop(c),
                    width: c.Size.Width,
                    height: c.Size.Height
                );

                if (range.Intersects(target))
                {
                    c.IsSelected = true;
                }
                else
                {
                    c.IsSelected = false;
                }
            }

            foreach (Wire wire in Wires)
            {
                foreach (Point pt in wire.Points)
                {
                    if (range.Contains(pt))
                    {
                        wire.IsSelected = true;
                    }
                    else
                    {
                        wire.IsSelected = false;
                    }
                }
            }
        }

        else if (PreviewObject != null)
        {
            if (PreviewObject is Component c)
            {
                Canvas.SetLeft(PreviewObject, CurrentMousePos.X);
                Canvas.SetTop(PreviewObject, CurrentMousePos.Y);
                c.Position = CurrentMousePos;
            }

            else if (PreviewObject is Wire wire && wire.Points.Count > 0)
            {
                if (wire.Nodes.Count >= 1)
                {
                    wire.RemoveLastNode();
                    wire.AddNode(
                        terminal: new Terminal(CurrentMousePos),
                        position: CurrentMousePos,
                        isOutputProvider: false
                    );
                }
            }
        }
    }


    private void OnPointerWheel(object? sender, PointerWheelEventArgs e)
    {
        CurrentMousePos = Utils.SnapPointToGrid(e.GetPosition(_canvas));

        if (PreviewObject != null)
        {
            if (PreviewObject is Component c)
            {
                Canvas.SetLeft(PreviewObject, CurrentMousePos.X);
                Canvas.SetTop(PreviewObject, CurrentMousePos.Y);
                c.Position = CurrentMousePos;
            }
        }
    }


    private void OnPointerEnter(object? sender, PointerEventArgs e)
    {
        CurrentMousePos = Utils.SnapPointToGrid(e.GetPosition(_canvas));

        if (PreviewObject != null)
        {
            if (!_canvas.Children.Contains(PreviewObject))
                _canvas.Children.Add(PreviewObject);

            if (PreviewObject is Wire wire && wire.Nodes.Count < 1)
            {
                wire.AddNode(
                    terminal: new Terminal(CurrentMousePos),
                    position: CurrentMousePos,
                    isOutputProvider: false
                );
            }

            else if (PreviewObject is Component c)
            {
                Canvas.SetLeft(PreviewObject, CurrentMousePos.X);
                Canvas.SetTop(PreviewObject, CurrentMousePos.Y);
                c.Position = CurrentMousePos;
            }
        }
    }


    private void OnPointerExit(object? sender, PointerEventArgs e)
    {
        if (PreviewObject != null)
        {
            if (PreviewObject is Component c)
            {
                _canvas.Children.Remove(c);
            }

            else if (PreviewObject is Wire wire)
            {
                wire.RemoveLastNode();
            }
        }
    }


    public void DropPreview() 
    {
        if (PreviewObject != null)
        {
            if (_canvas.Children.Contains(PreviewObject))
                _canvas.Children.Remove(PreviewObject);

            PreviewObject = null;
        }
    }


    public void DeleteSelected() 
    {
        foreach (Component c in Components.ToList())
        {
            if (c.IsSelected)
            {
                _canvas.Children.Remove(c);
                Components.Remove(c);
            }
        }

        foreach (Wire w in Wires.ToList())
        {
            if (w.IsSelected)
            {
                _canvas.Children.Remove(w);
                Wires.Remove(w);
            }
        }
    }
}

