using Avalonia;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using IRis.Services;
using IRis.ViewModels.Main.Canvas.Core;
using IRis.Models.Main.Canvas.Core;
using IRis.Base;
using System;


namespace IRis.Models.Core;


public partial class Preview : ManagerBase<Preview>
{
    public Point MouseOffset = new(0, 0);


    public void UpdatePosition(Point current)
    {
        SimulationService.SnapCollectionToPosition(Objects, current, MouseOffset);
    }


    public bool HasNewWire()
    {
        return Objects.Count == 1 && Objects[0] is WireViewModel;
    }


    public void Pick(ComponentViewModel c)
    {
        Ditch();
        Add(c);
        MouseOffset = new Point(c.Width / 2, c.Height / 2);
    }


    public void StartWireAt(TerminalViewModel t)
    {
        WireViewModel wire = new() 
        { 
            MainInput = new() { IsOrphan = true }, 
            MainOutput = new() { IsOrphan = true }
        };

        if (t.Type is TerminalType.Output) wire.MainInput = t;
        else if (t.Type is TerminalType.Input) wire.MainOutput = t;
        else return;

        Add(wire);
    }


    public void EndWireAt(TerminalViewModel t)
    {
        if (!HasNewWire()) return;
        var wire = (Objects[0] as WireViewModel)!;

        if (wire.MainInput.IsOrphan) wire.MainInput = t;
        else if (wire.MainOutput.IsOrphan) wire.MainOutput = t;
        else return;

        var sim = Simulation.GetInstance();
        wire.Opacity = 1.0;
        sim.Objects.Add(wire);
        Ditch();
    }


    public void CommitAll() 
    {
        Simulation sim = Simulation.GetInstance();

        foreach (var co in Objects)
        {
            // var clone = CloningService.Clone(co);
            co.Opacity = 1.0;
            sim.Add(co);
        }

        Ditch();
    }
}
