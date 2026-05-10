using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.Models.Core;


public abstract partial class CircuitObject : ObservableObject, ICloneable
{
    public readonly Guid Id = Guid.NewGuid();
    
    
    [ObservableProperty]
    private bool _isValid = true;
    
    [ObservableProperty]
    private bool _isSelected = false;
    
    [ObservableProperty]
    private bool _isPreview = true;

    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;


    // public bool IsSelected
    // {
    //     get => _isSelected;
    //     set 
    //     {
    //         _isSelected = value;
    //         InvalidateVisual();
    //     }
    // }
    //
    //
    // public bool IsPreview
    // {
    //     get => _isPreview;
    //     set
    //     {
    //         _isPreview = value;
    //         InvalidateVisual();
    //     }
    // }


    // public bool IsVabstract alid
    // {
    //     get => _isValid;
    //     set
    //     {
    //         _isValid = value;
    //         InvalidateVisual();
    //     }
    // }

    
    
    // NOTE: I think the XAML does this automatically too

    // public abstract bool HitTest(Point point);
    public abstract object Clone();
    // public abstract void Draw(DrawingContext ctx);
}