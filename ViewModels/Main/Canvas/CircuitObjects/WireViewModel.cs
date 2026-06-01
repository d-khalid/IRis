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

    public bool AllowFixing { get; set; } = true;


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
        if (e.PropertyName is nameof(TerminalViewModel.X) or nameof(TerminalViewModel.Y)
            && AllowFixing)
            Fix();
    }


    public void Redraw()
    {
        if (MainInput is null || MainOutput is null) return;

        Points.Clear();
        Points.Add(new Point((int)MainInput.X, (int)MainInput.Y));
        Points.Add(new Point((int)MainOutput.X, (int)MainOutput.Y));
    }


    public static AvaloniaList<Point> DrawWire(Point p1, Point p2)
    {
        // TODO: we implement this function later to work
        // with Simulation.ForbiddenMatrix

        AvaloniaList<Point> points = [];

        points.Add(p1);
        points.Add(new(p1.X, p2.Y));
        points.Add(p2);

        return points;
    }


    public void Fix()
    {
        // this math bellow was done by SHAHZAIB

        int inputIdx = Points.IndexOf(new(MainInput.X, MainInput.Y));
        int outputIdx = Points.IndexOf(new(MainOutput.X, MainOutput.Y));

        if (inputIdx == -1 && outputIdx > -1)
        {               // MainInput has moved out of position
            inputIdx = Points.Count - 1 - outputIdx;    // this is invalidPointtIndex
            int validPointIndex = ValidPointIndex(inputIdx);
            bool startsFromZero = inputIdx == 0;

            AvaloniaList<Point> points;
            points = startsFromZero 
                ? DrawWire(new(MainInput.X, MainInput.Y), Points[validPointIndex]) 
                : DrawWire(Points[validPointIndex], new(MainInput.X, MainInput.Y));

            if (startsFromZero)
            {
                Points.RemoveRange(0, validPointIndex + 1);
                Points.InsertRange(0, points);
            }

            else
            {
                Points.RemoveRange(validPointIndex, inputIdx - validPointIndex + 1);
                Points.AddRange(points);
            }
        }

        else if (inputIdx > -1 && outputIdx == -1)
        {
            outputIdx = Points.Count - 1 - inputIdx;    // this is invalidPointtIndex
            int validPointIndex = ValidPointIndex(outputIdx);
            bool startsFromZero = outputIdx == 0;

            AvaloniaList<Point> points;
            points = startsFromZero 
                ? DrawWire(new(MainOutput.X, MainOutput.Y), Points[validPointIndex]) 
                : DrawWire(Points[validPointIndex], new(MainOutput.X, MainOutput.Y));

            if (startsFromZero)
            {
                Points.RemoveRange(0, validPointIndex + 1);
                Points.InsertRange(0, points);
            }

            else
            {
                Points.RemoveRange(validPointIndex, outputIdx - validPointIndex + 1);
                Points.AddRange(points);
            }
        }

        else
        {
            // handled by SnapCollectionToPosition
        }

        int ValidPointIndex(int invalidPointIndex)
        {
            int idx1 = invalidPointIndex, idx2 = invalidPointIndex;
            int validPointIndex = Points.Count - 1 - invalidPointIndex;

            while (true)
            {
                idx2 = invalidPointIndex == 0 ? idx1 + 1 : idx1 - 1;

                if (idx2 > Points.Count - 1 || idx2 < 0) return validPointIndex;
                if (SimulationService.Distance(Points[idx2], Points[idx1]) > 30)
                {
                    return idx2;
                }

                idx1 = idx2;
            }
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
