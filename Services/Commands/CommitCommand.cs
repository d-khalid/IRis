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
            co.Opacity = 1.0;

            if (co is WireViewModel w && w.Points.Count == 0)
                w.Redraw();
        }

        Simulation.Get().Add(_collection);
    }


    public override void Undo() => Simulation.Get().Remove(_collection);
}
