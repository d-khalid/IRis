using Avalonia;
using Avalonia.Collections;
using IRis.ViewModels.Main.Canvas;

namespace IRis.Services.Commands;

public class MoveCommand(Point p1, Point p2, AvaloniaList<CircuitObjectViewModel> collection)
    : CommandBase
{
    private readonly Point _initial = p1;
    private readonly Point _final = p2;
    private readonly AvaloniaList<CircuitObjectViewModel> _collection = collection;

    public override void Execute() =>
        SimulationService.SnapCollectionToPosition(_collection, _final);

    public override void Undo() =>
        SimulationService.SnapCollectionToPosition(_collection, _initial);
}
