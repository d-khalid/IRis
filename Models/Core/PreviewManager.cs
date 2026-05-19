using IRis.ViewModels.Circuit;
using IRis.Models.Circuit.CircuitObjects.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;
using Avalonia;
using IRis.ViewModels.Circuit.CircuitObjects;
using IRis.Services;


namespace IRis.Models.Core;


public partial class PreviewManager : ObservableObject
{
    private static PreviewManager? _instance = null;
    public SimulationManager Simulation = SimulationManager.GetInstance();
    public ObservableCollection<CircuitObjectViewModel> Objects { get; } = [];

    [ObservableProperty] private bool _isVisible;
    public Point MouseOffset = new(0, 0);


    public PreviewManager()
    {
        if (_instance != null)
            throw new Exception("use GetInstance function instead pls.");
    }


    public static PreviewManager GetInstance()
    {
        if (_instance == null)
            _instance = new PreviewManager();

        return _instance;
    }


    public void SetVisible(bool state)
    {
        IsVisible = state;
    }


    public void Add(CircuitObjectViewModel obj)
    {
        Objects.Add(obj);
    }


    public bool HasObjects()
    {
        return Objects.Count > 0;
    }


    public void Ditch()
    {
        Objects.Clear();
    }


    public void Commit() 
    {
        foreach (var co in Objects)
        {
            if (co is ComponentViewModel c)
            {
                ComponentViewModel clone = CloningService.Clone(c);
                clone.Opacity = 1.0;
                Simulation.Objects.Add(clone);
            }
        }
    }
}
