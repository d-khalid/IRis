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
    public CircuitObjectViewModel? Partial = null;

    [ObservableProperty] private double _opacity;
    [ObservableProperty] private double _width;
    [ObservableProperty] private double _height;
    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;


    public void StartBox()
    {
        Ditch();
        Show();

        var sim = Simulation.GetInstance();
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


    public void EndBox()
    {
        Hide();
    }


    public void Highlight(CircuitObjectViewModel co)
    {
        Add(co);
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


    public void AddPartial(CircuitObjectViewModel co)
    {
        Partial = co;
        co.SelectionOpacity = 0.5;
        co.IsSelected = true;
    }


    public void DitchPartial()
    {
        if (Partial != null)
        {
            Partial.IsSelected = false;
            Partial.SelectionOpacity = 0.0;
            Partial = null;
        }
    }


    public void HidePartial()
    {
        if (Partial is not null)
        {
            Partial.IsSelected = false;
            Partial.SelectionOpacity = 0.0;
        }
    }


    public void ShowPartial()
    {
        if (Partial is not null)
        {
            Partial.SelectionOpacity = 0.5;
            Partial.IsSelected = true;
        }
    }


    public override void Add(CircuitObjectViewModel co)
    {
        base.Add(co);
        co.SelectionOpacity = 1.0;
        co.IsSelected = true;
    }


    public override void Remove(CircuitObjectViewModel co)
    {
        co.IsSelected = false;
        co.SelectionOpacity = 0.0;
        base.Remove(co);
    }


    public override void AddCollection(ObservableCollection<CircuitObjectViewModel> collection)
    {
        foreach (var co in collection) Add(co);
    }


    public override void RemoveCollection(ObservableCollection<CircuitObjectViewModel> collection)
    {
        foreach (var co in collection) co.IsSelected = false;
        base.RemoveCollection(collection);
    }
}
