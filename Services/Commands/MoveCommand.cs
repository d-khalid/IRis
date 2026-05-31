using Avalonia;
using IRis.ViewModels.Main.Canvas;
using IRis.Services.Singleton;
using Avalonia.Collections;


namespace IRis.Services.Commands;


public class MoveCommand(Point p1, Point p2, AvaloniaList<CircuitObjectViewModel> collection) : ICommand
{
    private readonly Point _initial = p1;
    private readonly Point _final = p2;
    private readonly AvaloniaList<CircuitObjectViewModel> _collection = collection;


    public void Execute() =>
        SimulationService.SnapCollectionToPosition(_collection, _final);

    public void Undo() =>
        SimulationService.SnapCollectionToPosition(_collection, _initial);
}
