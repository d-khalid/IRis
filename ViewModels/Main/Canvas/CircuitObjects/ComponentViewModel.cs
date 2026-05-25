using System;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects;


public abstract partial class ComponentViewModel : CircuitObjectViewModel
{
    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;
    [ObservableProperty] private double _width;
    [ObservableProperty] private double _height;
    [ObservableProperty] private double _rotation;

    partial void OnXChanged(double value) => UpdateTerminals();
    partial void OnYChanged(double value) => UpdateTerminals();
    partial void OnWidthChanged(double value) => UpdateTerminals();
    partial void OnHeightChanged(double value) => UpdateTerminals();
    partial void OnRotationChanged(double value) => UpdateTerminals();


    public bool Intersects(Rect rect)
    {
        return rect.Intersects(new Rect(X, Y, Width, Height));
    }


    public bool Contains(Point pt)
    {
        return new Rect (X, Y, Width, Height).Contains(pt);
    }


    public void PointerPressed()
    {
        Console.WriteLine("Component Pointer Press");
    }


    protected abstract void UpdateTerminals();
}
