using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels.Circuit.CircuitObjects;


public partial class ComponentViewModel : CircuitObjectViewModel
{
    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;
    [ObservableProperty] private double _width;
    [ObservableProperty] private double _height;
    [ObservableProperty] private double _opacity = 0.5;
}

