using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.ViewModels.Circuit.CircuitObjects.Core;


namespace IRis.ViewModels.Circuit.CircuitObjects;


public partial class WireViewModel : CircuitObjectViewModel
{
    public ObservableCollection<Point> Points { get; } = [];

    public TerminalViewModel MainInput;
    public TerminalViewModel MainOutput;


    public WireViewModel(TerminalViewModel mainInput, TerminalViewModel mainOutput)
    {
        MainInput = mainInput;
        MainOutput = mainOutput;
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
