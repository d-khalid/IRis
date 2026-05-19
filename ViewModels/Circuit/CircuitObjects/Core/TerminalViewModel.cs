using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Circuit.CircuitObjects.Core;


namespace IRis.ViewModels.Circuit.CircuitObjects.Core;


public partial class TerminalViewModel(Terminal model) : ObservableObject
{
    private readonly Terminal _model = model;
    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;
}

