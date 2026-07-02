using Avalonia;
using Avalonia.Collections;
using IRis.Services.Commands;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas;

namespace IRis.Services;

/// <summary>
/// Gets the objects from selection automatically and adds them to a list to
/// drag them with the mouse. Also implicitly throws away any hovers and selections.
/// </summary>
public class DragService(Selection selection, SimulationService simulationService)
{
    private readonly Selection _selection = selection;
    private readonly SimulationService _simulationService = simulationService;

    public AvaloniaList<CircuitObjectViewModel> Objects { get; } = [];
    public bool Used { get; set; } = false;

    private Point InitialPosition { get; set; } = new(0, 0);
    private Point FinalPosition { get; set; } = new(0, 0);
    private Point MouseOffset { get; set; } = new(0, 0);

    public void StartAt(Point position)
    {
        var collection = _selection.Objects;
        Objects.AddRange(collection);

        InitialPosition = _simulationService.GetMinPointInCollection(collection);
        MouseOffset = _simulationService.Difference(position, InitialPosition);
    }

    public void UpdatePositionTo(Point position)
    {
        if (!Used)
        {
            Used = true;
            _selection.UnHighlightAll();
        }

        FinalPosition = _simulationService.Difference(position, MouseOffset);
        _simulationService.SnapCollectionToPosition(Objects, FinalPosition, null);
    }

    public void Stop()
    {
        if (Used)
        {
            Used = false;
            _selection.Highlight(Objects);

            string name = Objects.Count == 1 ? "Move Object" : "Move Objects";
            CommandService.Execute(
                new MoveCommand(InitialPosition, FinalPosition, [.. Objects]) { Name = name }
            );
        }

        Objects.Clear();
    }

    public bool IsRunning() => Objects.Count > 0;
}
