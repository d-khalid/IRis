using System.Collections.Generic;
using Avalonia;
using Avalonia.Threading;
using System;
using IRis.ViewModels.Main.Canvas;
using Avalonia.Collections;


namespace IRis.Services.Singleton;


public partial class Simulation : SingletonCollection<Simulation>
{
    public bool Running = false;
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
        Selection.GetInstance().UnHighlightAll();
        Preview.GetInstance().Drop();
        Running = true;
        _timer.Start();
    }


    public void Stop()
    {
        _timer.Stop();
        Running = false;
        foreach (var co in Objects) co.Reset();
    }


    public void Add(CircuitObjectViewModel co) => Objects.Add(co);
    public void Add(AvaloniaList<CircuitObjectViewModel> coll) => Objects.AddRange(coll);
    public void Remove(CircuitObjectViewModel co) => Objects.Remove(co);
    public void Remove(AvaloniaList<CircuitObjectViewModel> coll) => Objects.RemoveAll(coll);
    public void Nuke() => Objects.Clear();
}
