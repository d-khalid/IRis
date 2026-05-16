using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels.Circuit.CircuitObjects;


public partial class ComponentViewModel : CircuitObjectViewModel
{
    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;
    [ObservableProperty] private double _width;
    [ObservableProperty] private double _height;
    [ObservableProperty] private double _rotation;


    public bool Intersects(Rect rect)
    {
        return rect.Intersects(new Rect(X, Y, Width, Height));
    }


    public bool Contains(Point pt)
    {
        return new Rect (X, Y, Width, Height).Contains(pt);
    }
}
