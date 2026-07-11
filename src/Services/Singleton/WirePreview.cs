using Avalonia;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;
using IRis.Services.Commands;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using IRis.ViewModels.Main.Canvas.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IRis.Services.Singleton;

public partial class WirePreview : ObservableObject
{
    [ObservableProperty]
    private WireViewModel? _wire = null;

    [ObservableProperty]
    private bool _isVisible = false;

    public AvaloniaList<Point> CommittedPoints { get; set; } = [];
    public AvaloniaList<Point> TemporaryPoints { get; set; } = [];

    private readonly ILogger<WirePreview> _logger = App.Current.Services.GetRequiredService<
        ILogger<WirePreview>
    >();

    public void StartAt(TerminalViewModel target)
    {
        WireViewModel wire = new()
        {
            MainInput = new() { IsOrphan = true },
            MainOutput = new() { IsOrphan = true },
        };

        if (target.Type is TerminalType.Output)
        {
            wire.MainInput = target;
            wire.MainInput.X = target.X;
            wire.MainInput.Y = target.Y;
        }
        else
        {
            wire.MainOutput = target;
            wire.MainOutput.X = target.X;
            wire.MainOutput.Y = target.Y;
        }

        wire.Opacity = 0.5;
        Wire = wire;

        CommittedPoints.Add(new Point(target.X, target.Y));
        Wire.Points.AddRange(CommittedPoints);

        Show();
    }

    public void UpdateTo(Point position)
    {
        if (Wire is null)
        {
            _logger.LogWarning("UpdateTo(): no wire is set currently.");
            return;
        }

        TemporaryPoints.Clear();

        var lastPt = CommittedPoints[^1];
        TemporaryPoints.Add(new(lastPt.X, position.Y));
        TemporaryPoints.Add(position);

        Wire.Points.Clear();
        Wire.Points.AddRange(CommittedPoints);
        Wire.Points.AddRange(TemporaryPoints);

        if (Wire.MainInput.IsOrphan)
        {
            Wire.MainInput.X = position.X;
            Wire.MainInput.Y = position.Y;
        }
        else if (Wire.MainOutput.IsOrphan)
        {
            Wire.MainOutput.X = position.X;
            Wire.MainOutput.Y = position.Y;
        }
    }

    public void Checkpoint()
    {
        if (Wire is null)
        {
            _logger.LogWarning("Checkpoint(): no wire is set currently.");
            return;
        }

        CommittedPoints.AddRange(TemporaryPoints);
        TemporaryPoints.Clear();

        Wire.Points.Clear();
        Wire.Points.AddRange(CommittedPoints);
    }

    public void Leave()
    {
        if (Wire is null)
        {
            _logger.LogWarning("Leave(): no wire is set currently.");
            return;
        }

        if (CommittedPoints.Count <= 1)
        {
            _logger.LogDebug("Leave(): no points were commited. Nuking wire without saving.");
        }
        else
        {
            Wire.Points.Clear();
            Wire.Points.AddRange(CommittedPoints);

            if (Wire.MainInput.IsOrphan)
            {
                Wire.MainInput.X = CommittedPoints[^1].X;
                Wire.MainInput.Y = CommittedPoints[^1].Y;
            }
            else if (Wire.MainOutput.IsOrphan)
            {
                Wire.MainOutput.X = CommittedPoints[^1].X;
                Wire.MainOutput.Y = CommittedPoints[^1].Y;
            }

            CommandService.Execute(new CommitCommand([Wire]) { Name = "Leave Wire" });
        }

        Nuke();
    }

    public void EndAt(TerminalViewModel target)
    {
        if (Wire is null)
        {
            _logger.LogWarning("WirePreview.EndAt(): no wire is set currently.");
            return;
        }

        Wire.Points.Clear();
        if (Wire.MainOutput.IsOrphan)
        {
            Wire.MainOutput = target;
            Wire.MainOutput.X = target.X;
            Wire.MainOutput.Y = target.Y;
        }
        else
        {
            Wire.MainInput = target;
            Wire.MainInput.X = target.X;
            Wire.MainInput.Y = target.Y;
        }

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
