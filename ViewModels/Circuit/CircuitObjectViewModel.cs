using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;


namespace IRis.ViewModels.Circuit;


public partial class CircuitObjectViewModel : ObservableObject
{
    [ObservableProperty] [JsonIgnore] private bool _isSelected = false;
    [ObservableProperty] [JsonIgnore] private double _opacity = 0.5;
    [ObservableProperty] [JsonIgnore] private double _selectionOpacity = 0.1;
}
