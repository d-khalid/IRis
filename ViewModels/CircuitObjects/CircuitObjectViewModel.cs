using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels.CircuitObjects;


public partial class CircuitObjectViewModel : ObservableObject
{
    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;
    [ObservableProperty] private double _width;
    [ObservableProperty] private double _height;
}

