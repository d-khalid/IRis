using Avalonia.Collections;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas;
using IRis.ViewModels.Main.Canvas.CircuitObjects;


namespace IRis.Services.Commands;


public class CommitCommand(AvaloniaList<CircuitObjectViewModel> collection) : CommandBase
{
    private readonly AvaloniaList<CircuitObjectViewModel> _collection = collection;


    public override void Execute()
    {
        foreach (var co in _collection)
        {
            if (co is WireViewModel w) w.Redraw();
            Simulation.Get().Add(co);
        }
    }


    public override void Undo()
    {
        foreach (var co in _collection)
            Simulation.Get().Remove(co);
    }
}
