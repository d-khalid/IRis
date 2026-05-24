using IRis.ViewModels.Circuit;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;
using Avalonia;
using IRis.ViewModels.Circuit.CircuitObjects;


namespace IRis.Models.Core;


public partial class Selection : ManagerBase<Selection>
{
    private Point _selectionBoxStartPt;

    [ObservableProperty] private double _width;
    [ObservableProperty] private double _height;
    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;


    public void StartBox()
    {
        Ditch();

        Simulation sim = (Simulation)Simulation.GetInstance();
        foreach (CircuitObjectViewModel co in sim.Objects)
        {
            if (co is ComponentViewModel c && c.Contains(sim.CurrentMousePos))
            {
                c.IsSelected = true;
            }
        }

        Show();
        _selectionBoxStartPt = sim.CurrentMousePos;
    }


    public void UpdateBox(ObservableCollection<CircuitObjectViewModel> selectables)
    {
        Simulation sim = Simulation.GetInstance();
        Point pt = sim.CurrentMousePos;

        Width = Math.Abs(_selectionBoxStartPt.X - pt.X);
        Height = Math.Abs(_selectionBoxStartPt.Y - pt.Y);
        X = Math.Min(_selectionBoxStartPt.X, pt.X);
        Y = Math.Min(_selectionBoxStartPt.Y, pt.Y);

        var selectionBounds = new Rect(X, Y, Width, Height);
        foreach (CircuitObjectViewModel co in selectables) 
        {
            if (co is ComponentViewModel c)
            {
                if (!c.IsSelected && c.Intersects(selectionBounds))
                {
                    Add(c);
                }

                else if (c.IsSelected && !c.Intersects(selectionBounds))
                {
                    Remove(c);
                }
            }
        }
    }


    public void FinishBox()
    {
        Hide();
    }


    public void Focus(CircuitObjectViewModel co)
    {
        Ditch();
        Add(co);
    }


    public override void Hide()
    {
        base.Hide();
        Width = 0;
        Height = 0;
    }


    public override void Add(CircuitObjectViewModel co)
    {
        base.Add(co);
        co.IsSelected = true;
    }


    public override void Remove(CircuitObjectViewModel co)
    {
        co.IsSelected = false;
        base.Remove(co);
    }


    public override void AddCollection(ObservableCollection<CircuitObjectViewModel> collection)
    {
        base.AddCollection(collection);
        foreach (var co in collection) co.IsSelected = true;
    }


    public override void RemoveCollection(ObservableCollection<CircuitObjectViewModel> collection)
    {
        foreach (var co in collection) co.IsSelected = false;
        base.RemoveCollection(collection);
    }
}
