using IRis.ViewModels.Circuit;
using IRis.Models.Circuit.CircuitObjects.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;
using Avalonia;
using IRis.ViewModels.Circuit.CircuitObjects;


namespace IRis.Models.Core;


public partial class SimulationManager : ObservableObject
{
    private static SimulationManager? _instance = null;
    [ObservableProperty] private Point _currentMousePos = new(0, 0);

    public ObservableCollection<CircuitObjectViewModel> Objects { get; } = [];
    public ObservableCollection<CircuitObjectViewModel> CopiedObjects { get; } = [];

    public ObservableCollection<CircuitObjectViewModel> DraggedObjects { get; } = [];
    public ObservableCollection<CircuitObjectViewModel> SelectedObjects { get; } = [];


    public SimulationManager()
    {
        if (_instance != null)
            throw new Exception("use GetInstance function instead pls.");
    }


    public static SimulationManager GetInstance()
    {
        if (_instance == null)
            _instance = new SimulationManager();

        return _instance;
    }


    public void SelectObject(CircuitObjectViewModel obj)
    {
        obj.IsSelected = true;
        SelectedObjects.Add(obj);
    }


    public void UnselectObject(CircuitObjectViewModel obj)
    {
        obj.IsSelected = false;
        SelectedObjects.Remove(obj);
    }


    public void UnselectAll()
    {
        SelectedObjects.Clear();

        foreach (CircuitObjectViewModel co in Objects)
        {
            co.IsSelected = false;
        }
    }


    public void SelectAll()
    {
        foreach (CircuitObjectViewModel co in Objects)
        {
            SelectObject(co);
        }
    }

    public CircuitObjectViewModel? GetContainerObject(Point pt)
    {
        foreach (CircuitObjectViewModel co in Objects)
        {
            if (co is ComponentViewModel c && c.Contains(pt))
                return co;
        }

        return null;
    }
}
