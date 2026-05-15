using IRis.ViewModels.Circuit;
using IRis.ViewModels.Circuit.CircuitObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;
using Avalonia;


namespace IRis.ViewModels;


public partial class SimulationViewModel : ViewModelBase
{
    private static SimulationViewModel? _instance = null;

    public ObservableCollection<ComponentViewModel> Components { get; } = [];
    public ObservableCollection<WireViewModel> Wires { get; } = [];

    [ObservableProperty] private CircuitObjectViewModel? _preview = null;
    [ObservableProperty] private Point _currentMousePos = new(0, 0);


    public SimulationViewModel()
    {
        if (_instance != null)
            throw new Exception("use GetInstance function instead pls.");
    }


    public static SimulationViewModel GetInstance()
    {
        if (_instance == null)
            _instance = new SimulationViewModel();

        return _instance;
    }


    public void UnselectAll()
    {
        foreach (var c in Components)
            if (c.IsSelected) c.IsSelected = false;

        foreach (var w in Wires)
            if (w.IsSelected) w.IsSelected = false;
    }
}
