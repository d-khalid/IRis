using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services.Commands;
using IRis.ViewModels.Main.Canvas;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IRis.Services.Singleton;

public partial class Simulation : ObservableObject
{
    [ObservableProperty]
    private bool _running = false;

    [ObservableProperty]
    private int _frequencyHz = 50;

    public AvaloniaList<CircuitObjectViewModel> Objects { get; } = [];
    private readonly HashSet<Point> _forbiddenMatrix = [];
    private readonly DispatcherTimer _timer;
    private readonly Selection _selection;
    private readonly Preview _preview;
    private readonly WirePreview _wirePreview;
    private readonly ILogger<Simulation> _logger;

    public Simulation(
        Selection selection,
        Preview preview,
        WirePreview wirePreview,
        ILogger<Simulation> logger
    )
    {
        _selection = selection;
        _preview = preview;
        _wirePreview = wirePreview;
        _logger = logger;

        // So this is what I need to get rid of
        _timer = new() { Interval = TimeSpan.FromMilliseconds(1000 / FrequencyHz) };

        _timer.Tick += (_, _) =>
        {
            foreach (var co in Objects)
                if (co is ComponentViewModel component)
                    component.Simulate();

            foreach (var co in Objects)
                if (co is WireViewModel wire)
                    wire.Simulate();
        };

        Running = true;
    }

    partial void OnFrequencyHzChanged(int value) =>
        _timer.Interval = TimeSpan.FromMilliseconds(1000 / value);

    partial void OnRunningChanged(bool value)
    {
        if (value)
        {
            _selection.UnHighlightAll();
            _preview.Drop();
            _wirePreview.Nuke();

            _timer.Start();
        }
        else
        {
            _timer.Stop();

            foreach (var co in Objects)
                co.Reset();
        }
    }

    public void UpdateForbiddenMatrix() => _forbiddenMatrix.Clear();

    public bool IsForbidden(Point pt) => _forbiddenMatrix.Contains(pt);

    public void Add(CircuitObjectViewModel co) => Objects.Add(co);

    public void Add(AvaloniaList<CircuitObjectViewModel> coll) => Objects.AddRange(coll);

    public void Nuke() => Objects.Clear();

    public void Remove(AvaloniaList<CircuitObjectViewModel> collection)
    {
        string name = collection.Count == 1 ? "Delete Object" : "Delete Objects";
        CommandService.Execute(new DeleteCommand(Objects, collection) { Name = name });
    }
}
