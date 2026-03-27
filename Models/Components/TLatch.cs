using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;
using System;

namespace IRis.Models.Components;

public class TLatch(
    double width = ComponentDefaults.DefaultMuxWidth * 5,
    double height = ComponentDefaults.DefaultMuxHeight)
    : LatchBase(width, height), IOutputProvider
{
    // Terminals:
    // 0: T   (left, top)
    // 1: EN  (left, bottom)
    // 2: Q   (right, top)
    // 3: Q'  (right, bottom)


    public void ComputeOutput()
    {
        var t = Terminals![0].Wire!.Value;
        var en = Terminals![1].Wire!.Value;

        if (en == LogicState.High)
        {
            if (t == LogicState.High)
            {
                // Toggle
                StoredStates["Q"] = StoredStates["Q"] == LogicState.High ? LogicState.Low : LogicState.High;
            }
            // else t==0 → hold
        }
        // else EN==Low → hold

        Terminals[2].Wire!.Value = StoredStates["Q"];
        Terminals[3].Wire!.Value = StoredStates["Q"] == LogicState.High ? LogicState.Low : LogicState.High;
    }

    public override void AddTerminalPoints(bool notMode = false)
    {
        Point SnapToGrid(Point pt)
        {
            double snapX = Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            double snapY = Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            return new Point(snapX, snapY);
        }

        // Inputs: T (top-left), EN (bottom-left)
        var tPos = new Point(-ComponentDefaults.TerminalWireLength, ComponentDefaults.TerminalSpacing * 1);
        var enPos = new Point(-ComponentDefaults.TerminalWireLength, ComponentDefaults.TerminalSpacing * 2);
        Terminals![0] = new Terminal(SnapToGrid(tPos), null!);
        Terminals![1] = new Terminal(SnapToGrid(enPos), null!);

        // Outputs: Q (top-right), Q' (bottom-right)
        var qPos = new Point(Width + ComponentDefaults.TerminalWireLength - 5, ComponentDefaults.TerminalSpacing * 1);
        var nqPos = new Point(Width + ComponentDefaults.TerminalWireLength - 5, ComponentDefaults.TerminalSpacing * 2);

        var qSnap = SnapToGrid(qPos);
        var nqSnap = SnapToGrid(nqPos);

        Terminals![2] = new Terminal(new Point(qPos.X, qSnap.Y), null!);
        Terminals![3] = new Terminal(new Point(nqPos.X, nqSnap.Y), null!);
    }

    internal override void DrawTerminalsAndLabels(DrawingContext ctx)
    {
        // Inputs
        string[] inLabels = { "T", "EN" };
        for (int i = 0; i < 2; i++)
        {
            ctx.DrawLine(ComponentDefaults.WirePen, Terminals![i].Position,
                new Point(0, Terminals[i].Position.Y));
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
                Terminals[i].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

            var text = inLabels[i].CreateFormattedText();

            ctx.DrawText(text, new Point(4.5, Terminals[i].Position.Y - 6));
        }

        // Outputs
        string[] outLabels = { "Q", "Q'" };
        for (int j = 2; j <= 3; j++)
        {
            ctx.DrawLine(ComponentDefaults.WirePen, Terminals![j].Position,
                new Point(Terminals[j].Position.X - ComponentDefaults.TerminalWireLength + 5, Terminals[j].Position.Y));
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
                Terminals[j].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

            var text = outLabels[j - 2].CreateFormattedText();

            ctx.DrawText(text, new Point(Width - 18, Terminals[j].Position.Y - 6));
        }
    }
}
