using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using IRis.Models.Core;

namespace IRis.Models;

public class ClipboardManager
{
    private Canvas _canvas;
    
    private List<Component> _clipboard;
    private List<Component> _pastePreviewComponents;
    private bool _isPastePreviewActive;
    private Point _lastPastePosition;
    
    private List<Component> _selectedComponents;
    private List<Component> _components;
    
    private Point _currentMousePos;
    private Func<Point> _getMousePos;

    public ClipboardManager(Canvas canvas, Func<Point> mousePosGetter, List<Component> components, List<Component> selectedComponents)
    {
        _canvas = canvas;
        _getMousePos = mousePosGetter;
        _components = components;
        _selectedComponents = selectedComponents;
    }



    private bool HandlePaste()
    {
        if (_isPastePreviewActive)
        {
            if (!_isPastePreviewActive) return true; // Termiante
 
            //_selectedComponents.Clear();
            _selectedComponents.AddRange(_pastePreviewComponents);
            _components.AddRange(_pastePreviewComponents);
        
            // Clear the paste preview so that the gates we have are stuck in the canvas
            _pastePreviewComponents.Clear();
            _isPastePreviewActive = false;
            //e.Handled = true;
            return true; // Terminate
        }
        return false;
    }

      // Adds selected components to clipboard
    public void CopySelected﻿(bool cutMode = false)
    {
        _clipboard.Clear();
        foreach (var component in _selectedComponents)
        {
            Component c = (Component)component.Clone();
            _clipboard.Add(c);

           
            
            // copy component positions
            if (c is Wire wire)
            {
                Canvas.SetLeft(c, -Canvas.GetLeft(component));
                Canvas.SetTop(c, -Canvas.GetTop(component));
            }
            else
            {
                Canvas.SetLeft(c, Canvas.GetLeft(component));
                Canvas.SetTop(c, Canvas.GetTop(component));
            }




            component.IsSelected = false;
        }
        if(cutMode) DeleteSelectedComponents();
        _selectedComponents.Clear();
    }
    
    public void CutSelected()
    {
        CopySelected(true);
    }
    
    public void StartPastePreview()
    {
        if (_clipboard.Count == 0) return;
        
        _pastePreviewComponents.Clear();
        _isPastePreviewActive = true;
        
        foreach (var component in _clipboard)
        {
            var clone = (Component)component.Clone();
            _pastePreviewComponents.Add(clone);
            _canvas.Children.Add(clone);
            
            //Canvas.SetZIndex(clone, int.MaxValue - 1); // Below selection but above others
            //clone.Opacity = 0.7;
        }
        UpdatePastePreviewPosition(LastMousePos);
    }
    
    public void CommitPaste()
    {
        if (!_isPastePreviewActive) return;
 
        //_selectedComponents.Clear();
        _selectedComponents.AddRange(_pastePreviewComponents);
        _components.AddRange(_pastePreviewComponents);
        
        // Clear the paste preview so that the gates we have are stuck in the canvas
        _pastePreviewComponents.Clear();
        _isPastePreviewActive = false;
    }
    
    private void UpdatePastePreviewPosition(Point position)
    {
        if (!_isPastePreviewActive) return;
        
        _lastPastePosition = position;
        //Point offset = CalculatePasteOffset();

        
        Point reference = new Point(Canvas.GetLeft(_clipboard[0]), Canvas.GetTop(_clipboard[0]));
        for (int i = 0; i < _pastePreviewComponents.Count; i++)
        {
            var original = _clipboard[i];
            var preview = _pastePreviewComponents[i];

            Console.WriteLine($"{Canvas.GetLeft(original)}, {Canvas.GetTop(original)}");
            Canvas.SetLeft(preview,  reference.X - Canvas.GetLeft(original) + LastMousePos.X);
            Canvas.SetTop(preview, reference.Y - Canvas.GetTop(original) + LastMousePos.Y);
        }
    }
    
    // public void ClearPastePreview()
    // {
    //     foreach (var component in _pastePreviewComponents)
    //     {
    //         _canvas.Children.Remove(component);
    //     }
    //     _pastePreviewComponents.Clear();
    //     _isPastePreviewActive = false;
    // }
}