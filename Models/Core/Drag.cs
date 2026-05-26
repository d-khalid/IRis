using IRis.Services;
using Avalonia;
using System.Collections.ObjectModel;
using IRis.ViewModels.Main.Canvas;
using System;
using IRis.Base;


namespace IRis.Models.Core;


public partial class Drag : ManagerBase<Drag>
{
    public bool Used = false;
    public Point MouseOffset;


    public void StartWith(ObservableCollection<CircuitObjectViewModel> collection)
    {
        AddCollection(collection);

        Point min = SimulationService.GetMinPointInCollection(collection);
        Point current = Simulation.GetInstance().CurrentMousePos;
        MouseOffset = SimulationService.Difference(current, min);
    }


    public void StartWith(CircuitObjectViewModel obj)
    {
        Add(obj);

        Point min = SimulationService.GetMinPointInCollection([obj]);
        Point current = Simulation.GetInstance().CurrentMousePos;
        MouseOffset = SimulationService.Difference(current, min);
    }


    public void UpdatePosition(Point current)
    {
        Used = true;
        SimulationService.SnapCollectionToPosition(Objects, current, MouseOffset);
    }


    public void End()
    {
        Used = false;
        Ditch();
    }
}
