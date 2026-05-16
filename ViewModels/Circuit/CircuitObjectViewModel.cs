using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels.Circuit;


public partial class CircuitObjectViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private double _opacity = 0.5;
}

