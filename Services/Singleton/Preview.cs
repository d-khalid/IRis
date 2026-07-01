using Avalonia;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services.Commands;
using IRis.ViewModels.Main.Canvas;
using IRis.ViewModels.Main.Canvas.CircuitObjects;

namespace IRis.Services.Singleton;

public partial class Preview : ObservableObject
{
    public AvaloniaList<CircuitObjectViewModel> Objects { get; } = [];
    private Point SavedMouseOffset { get; set; } = new(0, 0);

    [ObservableProperty]
    private bool _isVisible = false;

    public void UpdatePositionTo(Point position) =>
        SimulationService.SnapCollectionToPosition(Objects, position, SavedMouseOffset);

    public void Drop() => Objects.Clear();

    public void Pick(ComponentViewModel c) => Pick([c]);

    public void Pick(AvaloniaList<CircuitObjectViewModel> collection)
    {
        Objects.Clear();
        Objects.AddRange(collection);

        if (!IsVisible)
            IsVisible = true;
        foreach (var co in collection)
            co.Opacity = 0.5;

        Point min = SimulationService.GetMinPointInCollection(collection);
        Point max = SimulationService.GetMaxPointInCollection(collection);
        Point center = SimulationService.Average(min, max);

        SavedMouseOffset = SimulationService.Difference(center, min);
    }

    public void Commit()
    {
        var cloned = CloningService.Clone(Objects);
        string name = cloned.Count == 1 ? "Commit Object" : "Commit Objects";

        CommandService.Execute(new CommitCommand(cloned) { Name = name });
    }

    public void Show() => IsVisible = true;

    public void Hide() => IsVisible = false;

    public bool IsEmpty() => Objects.Count == 0;

    public void Nuke() => Objects.Clear();
}
