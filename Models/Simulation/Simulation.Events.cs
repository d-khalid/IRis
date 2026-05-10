using Avalonia.Controls;
using Avalonia.Input;
using System;

using IRis.Models.Core;
using IRis.Models.Components;


namespace IRis.Models;


public partial class Simulation
{   
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
                PreviewObject = null;
            }

            else if (PreviewObject is Wire wire)
            {
                wire.RemoveLastNode();
                Terminal? t = null;
                
                foreach (Component comp in Components)
                {
                    t = comp.GetTerminalHitTest(CurrentMousePos);
                    if (t != null) break;
                }

                if (t != null)
                {
                    wire.AddNode(
                        terminal: t,
                        position: CurrentMousePos,
                        isOutputProvider: t.IsOutputProvider
                    );

                    if (wire.Nodes.Count == 2) 
                    {
                        wire.IsPreview = false;
                        Wires.Add(wire);
                        PreviewObject = null;
                    }
                }

                wire.AddNode(
                    terminal: new Terminal(CurrentMousePos),
                    position: CurrentMousePos,
                    isOutputProvider: false
                );
            }
        }

        else if (!IsSimulating)     // selection logic
        {
            foreach (Component c in Components)
            {
                if (c.HitTest(CurrentMousePos) && !c.IsSelected)
                {
                    c.IsSelected = true;
                }

                else if (c.IsSelected)
                {
                    c.IsSelected = false;
                }
            }
        }
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {

    }
    

    private void OnPointerMoved(object? sender, PointerEventArgs e)
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


    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        
    }
}