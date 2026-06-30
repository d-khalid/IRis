using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services.Commands;
using IRis.ViewModels.Main.Canvas;

namespace IRis.Services.Singleton;

public partial class Simulation : SingletonCollection<Simulation>
{
    [ObservableProperty]
    private bool _running = true;

    private readonly HashSet<Point> _forbiddenMatrix = [];
    private readonly DispatcherTimer _timer;

    public Simulation()
    {
        _timer = new() { Interval = TimeSpan.FromMilliseconds(1000 / 100) };
        _timer.Tick += (_, _) =>
        {
            foreach (var co in Objects)
                co.Simulate();
        };
    }

    partial void OnRunningChanged(bool value)
    {
        if (value)
        {
            Selection.Get().UnHighlightAll();
            Preview.Get().Drop();
            WirePreview.Get().Nuke();

            _timer.Start();
        }
        else
        {
            _timer.Stop();

            foreach (var co in Objects)
                co.Reset();
        }
    }

    public void UpdateForbiddenMatrix()
    {
        _forbiddenMatrix.Clear();
    }

    public bool IsForbidden(Point pt) => _forbiddenMatrix.Contains(pt);

    public void Add(CircuitObjectViewModel co) => Objects.Add(co);

    public void Add(AvaloniaList<CircuitObjectViewModel> coll) => Objects.AddRange(coll);

    public void Remove(CircuitObjectViewModel co) => Objects.Remove(co);

    public void Remove(AvaloniaList<CircuitObjectViewModel> collection)
    {
        string name = collection.Count == 1 ? "Delete Object" : "Delete Objects";
        CommandService.Execute(new DeleteCommand(Objects, collection) { Name = name });
    }

    public void Nuke() => Objects.Clear();
}
