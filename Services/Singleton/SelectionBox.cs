using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia;
using IRis.ViewModels.Main.Canvas;
using System;


namespace IRis.Services.Singleton;


public partial class SelectionBox : SingletonBase<SelectionBox>
{
    private Point SelectionBoxStartPt { get; set; } = new(0, 0);
    [ObservableProperty] private bool _isVisible = false;
    [ObservableProperty] private double _x = 0;
    [ObservableProperty] private double _y = 0;
    [ObservableProperty] private double _width = 0;
    [ObservableProperty] private double _height = 0;


    public void StartAt(Point position)
    {
        Selection.Get().UnHighlightAll();
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
        var sel = Selection.Get();

        foreach (CircuitObjectViewModel co in Simulation.Get().Objects)
        {
            if (!co.IsSelected && co.Intersects(selectionBounds))
            {
                sel.Highlight(co);
            }

            else if (co.IsSelected && !co.Intersects(selectionBounds))
            {
                sel.UnHighlight(co);
            }
        }
    }


    public void Nuke()
    {
        X = Y = Width = Height = 0;
        IsVisible = false;
    }


    public bool Exists() => IsVisible;
}
