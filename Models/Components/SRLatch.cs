using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;
using System;

namespace IRis.Models.Components;

public class SRLatch(
    double width = ComponentDefaults.DefaultMuxWidth,
    double height = ComponentDefaults.DefaultMuxHeight)
    : LatchBase(width, height), IOutputProvider
{
    // Terminals layout (indexes):
    // 0: S (left, top)
    // 1: R (left, bottom)
    // 2: Q (right, top)
    // 3: Q' (right, bottom)


    public void ComputeOutput()
    {
        var s = Terminals![0].Wire!.Value;
        var r = Terminals![1].Wire!.Value;

        if (s == LogicState.High && r == LogicState.Low)
        {
            StoredStates["Q"] = LogicState.High;
        }
        else if (s == LogicState.Low && r == LogicState.High)
        {
            StoredStates["Q"] = LogicState.Low;
        }
        else if (s == LogicState.High && r == LogicState.High)
        {
            // Invalid condition for SR latch
            // PATCH: use dont care for now, but this needs to be fixed later
            Terminals[2].Wire!.Value = LogicState.DontCare;
            Terminals[3].Wire!.Value = LogicState.DontCare;
            return;
        }
        // else s=0, r=0 → hold previous state

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

        // Inputs: S (top-left), R (bottom-left)
        var sPos = new Point(-ComponentDefaults.TerminalWireLength, ComponentDefaults.TerminalSpacing * 1);
        var rPos = new Point(-ComponentDefaults.TerminalWireLength, ComponentDefaults.TerminalSpacing * 2);
        Terminals![0] = new Terminal(SnapToGrid(sPos), null!);
        Terminals![1] = new Terminal(SnapToGrid(rPos), null!);

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
        for (int i = 0; i < 2; i++)
        {
            ctx.DrawLine(ComponentDefaults.WirePen, Terminals![i].Position,
                new Point(0, Terminals[i].Position.Y));
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
                Terminals[i].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

            string label = i == 0 ? "S" : "R";
            var text = label.CreateFormattedText();

            ctx.DrawText(text, new Point(4.5, Terminals[i].Position.Y - 6));
        }

        // Outputs
        for (int j = 2; j <= 3; j++)
        {
            ctx.DrawLine(ComponentDefaults.WirePen, Terminals![j].Position,
                new Point(Terminals[j].Position.X - ComponentDefaults.TerminalWireLength + 5, Terminals[j].Position.Y));
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
                Terminals[j].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

            string label = j == 2 ? "Q" : "Q'";
            var text = label.CreateFormattedText();

            ctx.DrawText(text, new Point(Width - 18, Terminals[j].Position.Y - 6));
        }
    }
}
