using Avalonia.Collections;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using Microsoft.Extensions.DependencyInjection;

namespace IRis.Services.Commands;

public class CommitCommand(AvaloniaList<CircuitObjectViewModel> collection) : CommandBase
{
    private readonly AvaloniaList<CircuitObjectViewModel> _collection = collection;
    private readonly Simulation _simulation = App.Current.Services.GetRequiredService<Simulation>();

    public override void Execute()
    {
        foreach (var co in _collection)
        {
            co.Opacity = 1.0;

            if (co is WireViewModel w && w.Points.Count == 0)
                w.Redraw();
        }

        _simulation.Add(_collection);
    }

    public override void Undo() => _simulation.Remove(_collection);
}
