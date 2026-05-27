using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Base;
using IRis.Models.Main.Canvas;
using Newtonsoft.Json;


namespace IRis.ViewModels.Main.Canvas;


public abstract partial class CircuitObjectViewModel(CircuitObject model) : ObservableObject, ISimulatable
{
    [ObservableProperty] [property: JsonIgnore] private double _opacity = 0.5;
    [ObservableProperty] [property: JsonIgnore] private bool _isSelected = false;
    [ObservableProperty] [property: JsonIgnore] private double _selectionOpacity = 0.1;
    [ObservableProperty] [property: JsonIgnore] private int _zIndex = 0;

    protected CircuitObject Model { get; set; } = model;
    public void Simulate() => Model.Simulate();
    public void Reset() => Model.Reset();

    public abstract bool Contains(Point pt);
    public abstract bool Intersects(Rect rect);
}
