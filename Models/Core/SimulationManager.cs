using IRis.ViewModels.Circuit;
using IRis.ViewModels.Circuit.CircuitObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;
using Avalonia;


namespace IRis.Models.Core;


public partial class SimulationManager : ObservableObject
{
    private static SimulationManager? _instance = null;
    [ObservableProperty] private Point _currentMousePos = new(0, 0);

    public ObservableCollection<CircuitObjectViewModel> CircuitObjects { get; } = [];
    public ObservableCollection<CircuitObjectViewModel> CopiedObjects { get; } = [];
    public ObservableCollection<CircuitObjectViewModel> PreviewObjects { get; } = [];

    public ObservableCollection<CircuitObjectViewModel> DraggedObjects { get; } = [];
    public ObservableCollection<CircuitObjectViewModel> SelectedObjects { get; } = [];

    [ObservableProperty] private bool _isPreviewVisible;
    public Point PreviewMouseOffset = new(0, 0);


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

        foreach (CircuitObjectViewModel co in CircuitObjects)
        {
            co.IsSelected = false;
        }
    }


    public void SelectAll()
    {
        foreach (CircuitObjectViewModel co in CircuitObjects)
        {
            SelectObject(co);
        }
    }

    public CircuitObjectViewModel? GetContainerObject(Point pt)
    {
        foreach (CircuitObjectViewModel co in CircuitObjects)
        {
            if (co is ComponentViewModel c && c.Contains(pt))
                return co;
        }

        return null;
    }
}
