using System;
using Avalonia;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.ViewModels.Main.Canvas;

namespace IRis.Services.Singleton;

public partial class SelectionBox(Selection selection, Simulation simulation) : ObservableObject
{
    private Point SelectionBoxStartPt { get; set; } = new(0, 0);

    [ObservableProperty]
    private bool _isVisible = false;

    [ObservableProperty]
    private double _x = 0;

    [ObservableProperty]
    private double _y = 0;

    [ObservableProperty]
    private double _width = 0;

    [ObservableProperty]
    private double _height = 0;

    public AvaloniaList<CircuitObjectViewModel> Objects { get; } = [];
    private readonly Selection _selection = selection;
    private readonly Simulation _simulation = simulation;

    public void StartAt(Point position)
    {
        _selection.UnHighlightAll();
        SelectionBoxStartPt = position;
        IsVisible = true;
    }

    public void UpdateTo(Point position)
    {
        Width = Math.Abs(SelectionBoxStartPt.X - position.X);
        Height = Math.Abs(SelectionBoxStartPt.Y - position.Y);
        X = Math.Min(SelectionBoxStartPt.X, position.X);
        Y = Math.Min(SelectionBoxStartPt.Y, position.Y);

        var selectionBounds = new Rect(X, Y, Width, Height);

        foreach (CircuitObjectViewModel co in _simulation.Objects)
        {
            if (!co.IsSelected && co.Intersects(selectionBounds))
                _selection.Highlight(co);
            else if (co.IsSelected && !co.Intersects(selectionBounds))
                _selection.UnHighlight(co);
        }
    }

    public void Nuke()
    {
        X = Y = Width = Height = 0;
        IsVisible = false;
    }

    public bool Exists() => IsVisible;
}
