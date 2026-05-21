using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.ViewModels.Circuit.CircuitObjects.Core;
using System;


namespace IRis.ViewModels.Circuit.CircuitObjects;


public partial class WireViewModel : CircuitObjectViewModel
{
    public ObservableCollection<Point> Points { get; set; } = [];

    public TerminalViewModel MainInput { get; set; }
    public TerminalViewModel MainOutput { get; set; }


    public WireViewModel(TerminalViewModel mainInput, TerminalViewModel mainOutput)
    {
        MainInput = mainInput;
        MainOutput = mainOutput;

        MainInput.PropertyChanged += (_, _) => InvalidatePoints();
        MainOutput.PropertyChanged += (_, _) => InvalidatePoints();
    } 


    public bool Contains(Point pt)
    {
        return Points.Contains(pt);
    }


    public void InvalidatePoints()
    {
        Points.Clear();
        Points.Add(new Point((int)MainInput.X, (int)MainInput.Y));
        Points.Add(new Point((int)MainOutput.X, (int)MainOutput.Y));
    }
}
