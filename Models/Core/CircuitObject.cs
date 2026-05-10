using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Controls;


namespace IRis.Models.Core;


public abstract class CircuitObject : Control, ICustomHitTest, ISerializable, ICloneable
{
    public readonly Guid Id = Guid.NewGuid();
    private bool _isSelected = false;
    private bool _isPreview = true;
    private bool _isValid = true;


    public bool IsSelected
    {
        get => _isSelected;
        set 
        {
            _isSelected = value;
            InvalidateVisual();
        }
    }


    public bool IsPreview
    {
        get => _isPreview;
        set
        {
            _isPreview = value;
            InvalidateVisual();
        }
    }


    public bool IsValid
    {
        get => _isValid;
        set
        {
            _isValid = value;
            InvalidateVisual();
        }
    }


    public abstract bool HitTest(Point point);
    public abstract void Serialize();
    public abstract object Clone();
    public abstract void Draw(DrawingContext ctx);
}