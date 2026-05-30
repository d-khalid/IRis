using IRis.Services.Singleton;
using Avalonia;
using Avalonia.Collections;
using IRis.ViewModels.Main.Canvas;


namespace IRis.Services;


/// <summary>
/// Gets the objects from selection automatically and adds them to a list to 
/// drag them with the mouse. Also implicitly throws away any hovers and selections.
/// </summary>
public static class DragService
{
    public static AvaloniaList<CircuitObjectViewModel> Objects { get; } = [];
    private static Point SavedMouseOffset { get; set; } = new(0, 0);
    public static bool Used { get; set; } = false;


    public static void StartAt(Point position)
    {
        var collection = Selection.GetInstance().Objects;
        Objects.AddRange(collection);

        Point min = SimulationService.GetMinPointInCollection(collection);
        SavedMouseOffset = SimulationService.Difference(position, min);
    }


    public static void UpdatePositionTo(Point position)
    {
        if (!Used)
        {
            Used = true;
            Selection.GetInstance().UnHighlightAll();
        }

        SimulationService.SnapCollectionToPosition(Objects, position, SavedMouseOffset);
    }


    public static void Stop()
    {
        if (Used)
        {
            Used = false;
            Selection.GetInstance().Highlight(Objects);
        }

        Objects.Clear();
    }


    public static bool IsRunning() => Objects.Count > 0;
}
