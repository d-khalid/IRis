using System.Collections.ObjectModel;
using Avalonia;
using IRis.ViewModels.Main.Canvas.Core;
using System.ComponentModel;
using IRis.Models.CircuitObjects;
using IRis.Models.Core;
using IRis.Services;
using IRis.Services.Singleton;
using Newtonsoft.Json;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Collections;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects;


public partial class WireViewModel() : CircuitObjectViewModel(new Wire())
{
    [ObservableProperty] 
    private AvaloniaList<Point> _points = [];


    [ObservableProperty] private TerminalViewModel _mainInput = null!;
    partial void OnMainInputChanged(TerminalViewModel value)
    {
        (Model as Wire)!.MainInput = value.GetModel();
        value.PropertyChanged += OnTerminalPropertyChanged;
    }


    [ObservableProperty] private TerminalViewModel _mainOutput = null!;
    partial void OnMainOutputChanged(TerminalViewModel value)
    {
        (Model as Wire)!.MainOutput = value.GetModel();
        value.PropertyChanged += OnTerminalPropertyChanged;
    }


    private void OnTerminalPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(TerminalViewModel.X) or nameof(TerminalViewModel.Y))
            Fix();
    }


    public void Redraw()
    {
        if (MainInput is null || MainOutput is null) return;

        Points.Clear();
        Points.Add(new Point((int)MainInput.X, (int)MainInput.Y));
        Points.Add(new Point((int)MainOutput.X, (int)MainOutput.Y));
    }


    private static AvaloniaList<Point> DrawWire(Point p1, Point p2)
    {
        // TODO: we implement this function later to work
        // with Simulation.ForbiddenMatrix

        AvaloniaList<Point> points = [];

        if (p1.X != p2.X && p1.Y != p2.Y)
            points.Add(new(p1.X, p2.Y));

        points.Add(p2);

        return points;
    }


    public void Fix()
    {
        // summary: invalidate last 2 points of the wire
        // by last, i mean the invalid one, and closest to invalid one
        // O(p) btw, where p is Points.Count

        if (Points.Count == 2)
        {
            Points.Clear();
            
            var points = DrawWire(
                new(MainInput.X, MainInput.Y), new(MainOutput.X, MainOutput.Y)
            );

            points.Insert(0, new(MainInput.X, MainInput.Y));
            Points.AddRange(points);
            return;
        }

        int inputIdx = Points.IndexOf(new(MainInput.X, MainInput.Y));
        int outputIdx = Points.IndexOf(new(MainOutput.X, MainOutput.Y));

        if (inputIdx == -1 && outputIdx > -1)   // mainInput is the issue
        {
            int invalidPointtIndex = Points.Count - 1 - outputIdx;
            bool invalidIsStart = invalidPointtIndex == 0;
            int validPointIndex = invalidIsStart
                ? invalidPointtIndex + 2
                : invalidPointtIndex - 2;

            if (invalidIsStart)
            {
                var points = DrawWire(new(MainInput.X, MainInput.Y), Points[validPointIndex]);

                points.Insert(0, new(MainInput.X, MainInput.Y));
                Points.RemoveRange(0, validPointIndex + 1);
                Points.InsertRange(0, points);
            }

            else
            {
                var points = DrawWire(Points[validPointIndex], new(MainInput.X, MainInput.Y));
                Points.RemoveRange(validPointIndex + 1, Points.Count - invalidPointtIndex + 1);
                Points.AddRange(points);
            }
        }

        else if (inputIdx > -1 && outputIdx == -1)  // mainOutput is the issue
        {
            int invalidPointtIndex = Points.Count - 1 - inputIdx;
            bool invalidIsStart = invalidPointtIndex == 0;
            int validPointIndex = invalidIsStart
                ? invalidPointtIndex + 2
                : invalidPointtIndex - 2;

            if (invalidIsStart)
            {
                var points = DrawWire(new(MainOutput.X, MainOutput.Y), Points[validPointIndex]);

                points.Insert(0, new(MainOutput.X, MainOutput.Y));
                Points.RemoveRange(0, validPointIndex + 1);
                Points.InsertRange(0, points);
            }

            else
            {
                var points = DrawWire(Points[validPointIndex], new(MainOutput.X, MainOutput.Y));
                Points.RemoveRange(validPointIndex + 1, Points.Count - invalidPointtIndex + 1);
                Points.AddRange(points);
            }
        }

        else if (inputIdx == -1 && outputIdx == -1)
        {
            // just move points by the offset their terminals did
            Console.WriteLine("nope");
            return;
        }
    }


    public void SetOrphanTo(TerminalViewModel target)
    {
        if (MainInput.IsOrphan) MainInput = target;
        else if (MainOutput.IsOrphan) MainOutput = target;
        else Console.WriteLine("SetOrphanTo(): could not find any orphan in wire.");
    }


    public override bool Contains(Point pt)
    {
        return Points.Contains(pt);
    }


    public override bool Intersects(Rect rect)
    {
        for (int i = 0; i < Points.Count - 1; i++)
        {
            if (rect.Contains(Points[i]) || rect.Contains(Points[i + 1]))
                return true;
            else if (new Rect(Points[i], Points[i + 1]).Inflate(6).Intersects(rect))
                return true;
        }

        return false;
    }


    public void PointerPressed()
    {
        if (!IsSelected) Selection.Get().Highlight(this);
    }


    public void PointerEntered()
    {
        if (!WirePreview.Get().IsEmpty() || !Preview.Get().IsEmpty() || 
            DragService.IsRunning()) 
            return;

        if (!IsSelected) HoverEffectService.On(this);
    }


    public void PointerExited()
    {
        if (!WirePreview.Get().IsEmpty() || !Preview.Get().IsEmpty() || 
            DragService.IsRunning()) 
            return;

        if (!IsSelected && HoverEffectService.IsRunning()) 
            HoverEffectService.Stop();
    }
}
