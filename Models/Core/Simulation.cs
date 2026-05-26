using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using Avalonia;
using IRis.Base;
using Avalonia.Threading;
using System;


namespace IRis.Models.Core;


public partial class Simulation : ManagerBase<Simulation>
{
    public bool Running = false;
    [ObservableProperty] private Point _currentMousePos = new(0, 0);
    private readonly HashSet<Point> _forbiddenMatrix = [];
    private readonly DispatcherTimer _timer;


    public Simulation()
    {
        _timer = new() { Interval = TimeSpan.FromMilliseconds(100) };
        _timer.Tick += (_, _) =>
        {
            foreach (var co in Objects) co.Simulate();
        };
    }


    public void UpdateForbiddenMatrix()
    {
        _forbiddenMatrix.Clear();
    }


    public bool IsForbidden(Point pt)
    {
        return _forbiddenMatrix.Contains(pt);
    }


    public void Start()
    {
        Running = true;
        _timer.Start();
    }


    public void End()
    {
        _timer.Stop();
        Running = false;
    }
}
