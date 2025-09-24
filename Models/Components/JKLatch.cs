using System;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;

namespace IRis.Models.Components;

public class JKLatch : Component, IOutputProvider
{
    // Terminals:
    // 0: J   (left, top)
    // 1: K   (left, bottom)
    // 2: Q   (right, top)
    // 3: Q'  (right, bottom)


    public JKLatch(double width = ComponentDefaults.DefaultMuxWidth * 5,
                   double height = ComponentDefaults.DefaultMuxHeight)
        : base(width, height)
    {
        Width  = 5 * ComponentDefaults.TerminalSpacing;
        Height = 3 * ComponentDefaults.TerminalSpacing + ComponentDefaults.GridSpacing;

        Terminals = new Terminal[4];
        AddTerminalPoints();
        IsHitTestVisible = true;
        
        // Dictionary entry for state
        StoredStates["Q"] = LogicState.Low;
    }

    public void ComputeOutput()
    {
        var j = Terminals![0].Wire!.Value;
        var k = Terminals![1].Wire!.Value;

        if (j == LogicState.Low && k == LogicState.Low)
        {
            // Hold previous
        }
        else if (j == LogicState.High && k == LogicState.Low)
        {
            StoredStates["Q"] = LogicState.High;
        }
        else if (j == LogicState.Low && k == LogicState.High)
        {
            StoredStates["Q"] = LogicState.Low;
        }
        else if (j == LogicState.High && k == LogicState.High)
        {
            // Toggle
            StoredStates["Q"] = StoredStates["Q"] == LogicState.High ? LogicState.Low : LogicState.High;
        }

        Terminals[2].Wire!.Value = StoredStates["Q"];
        Terminals[3].Wire!.Value = StoredStates["Q"] == LogicState.High ? LogicState.Low : LogicState.High;
    }

    public override void Draw(DrawingContext ctx)
    {
        ctx.DrawRectangle(ComponentDefaults.GateFillBrush,
            ComponentDefaults.GatePen,
            new Rect(0, 0, Width, Height));

        DrawTerminalsAndLabels(ctx);
        base.Draw(ctx);
    }

    public override void DrawSelection(DrawingContext ctx)
    {
        double expandX = ComponentDefaults.TerminalWireLength + ComponentDefaults.TerminalRadius;
        double expandY = ComponentDefaults.TerminalRadius;

        ctx.DrawRectangle(
            ComponentDefaults.SelectionBrush,
            ComponentDefaults.SelectionPen,
            new Rect(
                -expandX,
                -expandY,
                Bounds.Width + 2 * expandX,
                Bounds.Height + 2 * expandY)
        );
    }

    public override void AddTerminalPoints(bool notMode = false)
    {
        Point SnapToGrid(Point pt)
        {
            double snapX = Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            double snapY = Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            return new Point(snapX, snapY);
        }

        // Inputs: J (top-left), K (bottom-left)
        var jPos = new Point(-ComponentDefaults.TerminalWireLength, ComponentDefaults.TerminalSpacing * 1);
        var kPos = new Point(-ComponentDefaults.TerminalWireLength, ComponentDefaults.TerminalSpacing * 2);
        Terminals![0] = new Terminal(SnapToGrid(jPos), null!);
        Terminals![1] = new Terminal(SnapToGrid(kPos), null!);

        // Outputs: Q (top-right), Q' (bottom-right)
        var qPos  = new Point(Width + ComponentDefaults.TerminalWireLength - 5, ComponentDefaults.TerminalSpacing * 1);
        var nqPos = new Point(Width + ComponentDefaults.TerminalWireLength - 5, ComponentDefaults.TerminalSpacing * 2);

        var qSnap  = SnapToGrid(qPos);
        var nqSnap = SnapToGrid(nqPos);

        Terminals![2] = new Terminal(new Point(qPos.X,  qSnap.Y),  null!);
        Terminals![3] = new Terminal(new Point(nqPos.X, nqSnap.Y), null!);
    }

    private void DrawTerminalsAndLabels(DrawingContext ctx)
    {
        // Inputs
        string[] inLabels = { "J", "K" };
        for (int i = 0; i < 2; i++)
        {
            ctx.DrawLine(ComponentDefaults.WirePen, Terminals![i].Position,
                new Point(0, Terminals[i].Position.Y));
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
                Terminals[i].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

            var text = new FormattedText(
                inLabels[i],
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                ComponentDefaults.LabelTypeface,
                ComponentDefaults.LabelSize,
                ComponentDefaults.LabelBrush
            );
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

            var text = new FormattedText(
                outLabels[j - 2],
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                ComponentDefaults.LabelTypeface,
                ComponentDefaults.LabelSize,
                ComponentDefaults.LabelBrush
            );
            ctx.DrawText(text, new Point(Width - 18, Terminals[j].Position.Y - 6));
        }
    }
}
