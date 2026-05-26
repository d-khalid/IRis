using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Base;
using IRis.Models.Main.Canvas;
using Newtonsoft.Json;


namespace IRis.ViewModels.Main.Canvas;


public partial class CircuitObjectViewModel(CircuitObject model) : ObservableObject, ISimulatable
{
    [ObservableProperty] [property: JsonIgnore] private double _opacity = 0.5;
    [ObservableProperty] [property: JsonIgnore] private bool _isSelected = false;
    [ObservableProperty] [property: JsonIgnore] private double _selectionOpacity = 0.1;
    [ObservableProperty] [property: JsonIgnore] private int _zIndex = 0;

    protected readonly CircuitObject Model = model;
    public void Simulate() => Model.Simulate();
}
