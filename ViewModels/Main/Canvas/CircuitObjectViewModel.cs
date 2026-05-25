using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;


namespace IRis.ViewModels.Main.Canvas;


public partial class CircuitObjectViewModel : ObservableObject
{
    [ObservableProperty] [property: JsonIgnore] private double _opacity = 0.5;
    [ObservableProperty] [property: JsonIgnore] private bool _isSelected = false;
    [ObservableProperty] [property: JsonIgnore] private double _selectionOpacity = 0.1;
}
