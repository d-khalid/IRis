using Avalonia;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;
using IRis.Services.Commands;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.Services.Singleton;

public partial class WirePreview : ObservableObject
{
    [ObservableProperty]
    private WireViewModel? _wire = null;

    [ObservableProperty]
    private bool _isVisible = false;

    public AvaloniaList<Point> CommittedPoints { get; set; } = [];
    public AvaloniaList<Point> TemporaryPoints { get; set; } = [];

    public void StartAt(TerminalViewModel target)
    {
        WireViewModel wire = new()
        {
            MainInput = new() { IsOrphan = true },
            MainOutput = new() { IsOrphan = true },
        };

        if (target.Type is TerminalType.Output)
            wire.MainInput = target;
        else if (target.Type is TerminalType.Input)
            wire.MainOutput = target;
        else
            return;

        wire.Opacity = 0.5;
        Wire = wire;

        CommittedPoints.Add(new Point(target.X, target.Y));
        Wire!.Points.AddRange(CommittedPoints);

        Show();
    }

    public void UpdateTo(Point position)
    {
        TemporaryPoints.Clear();

        // the math bellow for L-shaped wire routing was done by SHAHZAIB

        var lastPt = CommittedPoints[^1];
        TemporaryPoints.Add(new(lastPt.X, position.Y));
        TemporaryPoints.Add(position);

        Wire!.Points.Clear();
        Wire.Points.AddRange(CommittedPoints);
        Wire.Points.AddRange(TemporaryPoints);
    }

    public void Checkpoint()
    {
        CommittedPoints.AddRange(TemporaryPoints);
        TemporaryPoints.Clear();

        Wire!.Points.Clear();
        Wire.Points.AddRange(CommittedPoints);
    }

    public void Leave()
    {
        Wire!.Points.Clear();
        Wire.Points.AddRange(CommittedPoints);

        CommandService.Execute(new CommitCommand([Wire]) { Name = "Leave Wire" });

        Nuke();
    }

    public void Pick(WireViewModel wire)
    {
        Wire = wire;
        CommittedPoints = Wire.Points;
    }

    public void EndAt(TerminalViewModel target)
    {
        if (Wire is null)
            return;
        Wire.Points.Clear();
        Wire.SetOrphanTo(target);

        CommittedPoints.AddRange(TemporaryPoints);
        Wire.Points.AddRange(CommittedPoints);

        CommandService.Execute(new CommitCommand([Wire]) { Name = "Commit Wire" });

        Nuke();
    }

    public void Nuke()
    {
        Hide();

        Wire = null;
        TemporaryPoints.Clear();
        CommittedPoints.Clear();
    }

    public bool IsEmpty() => Wire is null;

    public bool WireValidForCommit() => Wire is not null && Wire.Points.Count > 1;

    public void Show() => IsVisible = true;

    public void Hide() => IsVisible = false;
}
