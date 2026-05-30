using Avalonia;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using IRis.ViewModels.Main.Canvas.Core;
using IRis.Models.Main.Canvas.Core;
using Avalonia.Collections;
using IRis.ViewModels.Main.Canvas;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.Services.Singleton;


/// <summary>
/// Handles previewing components through Pick(), Commit() and wires through 
/// StartWireAt(), EndWireAt(). Commit() also handles entire circuit previews.
/// Starting position for a preview is none, use the UpdatePositionTo() method.
/// </summary>
public partial class Preview : SingletonCollection<Preview>
{
    private Point SavedMouseOffset { get; set; } = new(0, 0);
    [ObservableProperty] private bool _isVisible = false;


    public void UpdatePositionTo(Point position) =>
        SimulationService.SnapCollectionToPosition(Objects, position, SavedMouseOffset);


    public bool HasNewWire() => Objects.Count == 1 && Objects[0] is WireViewModel;
    public void Drop() => Objects.Clear();
    public void Pick(ComponentViewModel c) => Pick([c]);


    public void Pick(AvaloniaList<CircuitObjectViewModel> collection)
    {
        Objects.Clear();
        Objects.AddRange(collection);

        if (!IsVisible) IsVisible = true;

        Point min = SimulationService.GetMinPointInCollection(collection);
        Point max = SimulationService.GetMaxPointInCollection(collection);
        Point center = SimulationService.Average(min, max);

        SavedMouseOffset = SimulationService.Difference(center, min);
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

        Objects.Add(wire);
    }


    public void EndWireAt(TerminalViewModel t)
    {
        if (!HasNewWire()) return;
        var wire = (Objects[0] as WireViewModel)!;

        if (wire.MainInput.IsOrphan) wire.MainInput = t;
        else if (wire.MainOutput.IsOrphan) wire.MainOutput = t;
        else return;

        wire.Opacity = 1.0;
        Simulation.GetInstance().Add(wire);
        Objects.Clear();
    }


    public void Commit()
    {
        Simulation sim = Simulation.GetInstance();
        var cloned = CloningService.Clone(Objects);

        foreach (var co in cloned)
        {
            if (co is WireViewModel w)
                w.Redraw();    // force wire redraw

            co.Opacity = 1.0;
            sim.Add(co);
        }
    }


    public void Show() => IsVisible = true;
    public void Hide() => IsVisible = false;
    public bool IsEmpty() => Objects.Count == 0;
    public void Nuke() => Objects.Clear();
}
