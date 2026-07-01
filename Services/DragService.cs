using Avalonia;
using Avalonia.Collections;
using IRis.Services.Commands;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas;
using Microsoft.Extensions.DependencyInjection;

namespace IRis.Services;

/// <summary>
/// Gets the objects from selection automatically and adds them to a list to
/// drag them with the mouse. Also implicitly throws away any hovers and selections.
/// </summary>
public static class DragService
{
    public static AvaloniaList<CircuitObjectViewModel> Objects { get; } = [];
    public static bool Used { get; set; } = false;
    private static readonly Selection _selection =
        App.Current.Services.GetRequiredService<Selection>();

    private static Point InitialPosition { get; set; } = new(0, 0);
    private static Point FinalPosition { get; set; } = new(0, 0);
    private static Point MouseOffset { get; set; } = new(0, 0);

    public static void StartAt(Point position)
    {
        var collection = _selection.Objects;
        Objects.AddRange(collection);

        InitialPosition = SimulationService.GetMinPointInCollection(collection);
        MouseOffset = SimulationService.Difference(position, InitialPosition);
    }

    public static void UpdatePositionTo(Point position)
    {
        if (!Used)
        {
            Used = true;
            _selection.UnHighlightAll();
        }

        FinalPosition = SimulationService.Difference(position, MouseOffset);
        SimulationService.SnapCollectionToPosition(Objects, FinalPosition, null);
    }

    public static void Stop()
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

    public static bool IsRunning() => Objects.Count > 0;
}
