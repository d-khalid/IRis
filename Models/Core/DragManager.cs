using IRis.ViewModels.Circuit;
using IRis.Models.Circuit.CircuitObjects.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;
using Avalonia;
using IRis.ViewModels.Circuit.CircuitObjects;
using IRis.Services;


namespace IRis.Models.Core;


public partial class DragManager : ObservableObject
{
    private static DragManager? _instance = null;
    public SimulationManager Simulation = SimulationManager.GetInstance();
    public PreviewManager Preview = PreviewManager.GetInstance();

    public ObservableCollection<CircuitObjectViewModel> Objects { get; } = [];
    public bool Used = false;


    public DragManager()
    {
        if (_instance != null)
            throw new Exception("use GetInstance function instead pls.");
    }


    public static DragManager GetInstance()
    {
        if (_instance == null)
            _instance = new DragManager();

        return _instance;
    }


    public void Add(CircuitObjectViewModel obj)
    {
        Objects.Add(obj);
        Used = false;
    }


    public void AddCollection(ObservableCollection<CircuitObjectViewModel> collection)
    {
        foreach (var co in collection) Objects.Add(co);
    }


    public void Update()
    {
        Used = true;

        SimulationService.SnapCollectionToPosition(
            Objects,
            Simulation.CurrentMousePos,
            Preview.MouseOffset
        );
    }


    public bool HasObjects()
    {
        return Objects.Count > 0;
    }


    public void Ditch()
    {
        Objects.Clear();
    }
}
