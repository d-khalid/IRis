using IRis.ViewModels.Circuit;
using IRis.Models.Circuit.CircuitObjects.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;
using Avalonia;
using IRis.ViewModels.Circuit.CircuitObjects;
using IRis.Services;
using Avalonia.Input;
using Avalonia.Controls;


namespace IRis.Models.Core;


public partial class SelectionManager : ObservableObject
{
    private static SelectionManager? _instance = null;
    public SimulationManager Simulation = SimulationManager.GetInstance();

    public ObservableCollection<CircuitObjectViewModel> Objects { get; } = [];
    private Point _selectionBoxStartPt;

    [ObservableProperty] private bool _isVisible;
    [ObservableProperty] private double _width;
    [ObservableProperty] private double _height;
    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;


    public SelectionManager()
    {
        if (_instance != null)
            throw new Exception("use GetInstance function instead pls.");
    }


    public static SelectionManager GetInstance()
    {
        if (_instance == null)
            _instance = new SelectionManager();

        return _instance;
    }


    public void Start()
    {
        Simulation.UnselectAll();

        var c = Simulation.GetContainerObject(Simulation.CurrentMousePos);
        if (c != null) Simulation.SelectObject(c);

        IsVisible = true;
        _selectionBoxStartPt = Simulation.CurrentMousePos;
    }


    public void Update()
    {
        Point pt = Simulation.CurrentMousePos;
        Width = Math.Abs(_selectionBoxStartPt.X - pt.X);
        Height = Math.Abs(_selectionBoxStartPt.Y - pt.Y);
        X = Math.Min(_selectionBoxStartPt.X, pt.X);
        Y = Math.Min(_selectionBoxStartPt.Y, pt.Y);

        var selectionBounds = new Rect(X, Y, Width, Height);
        foreach (CircuitObjectViewModel co in Simulation.Objects) 
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


    public void Add(CircuitObjectViewModel obj)
    {
        Simulation.SelectObject(obj);
        Objects.Add(obj);
    }


    public void Remove(CircuitObjectViewModel obj)
    {
        Simulation.UnselectObject(obj);
        Objects.Remove(obj);
    }


    public bool HasObjects()
    {
        return Objects.Count > 0;
    }


    public void Ditch()
    {
        Objects.Clear();
        Simulation.UnselectAll();
    }


    public void Show()
    {
        IsVisible = true;
    }


    public void Hide()
    {
        IsVisible = false;
        Width = 0;
        Height = 0;
    }
}
