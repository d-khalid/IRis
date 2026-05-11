using System;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;

namespace IRis.Models.Components;

public class Demultiplexer : Component, IOutputProvider
{

    public Demultiplexer(int selectionLineCount, double width = ComponentDefaults.DefaultMuxWidth,
        double height = ComponentDefaults.DefaultMuxHeight)
        : base(width, height)
    {
        SelectionLineCount = selectionLineCount;
        OutputLineCount = (int)Math.Pow(2, SelectionLineCount);

        // Geometry consistent with Multiplexer, but mirrored (1 input, many outputs)
        Width  = (SelectionLineCount + 1) * ComponentDefaults.TerminalSpacing;
        Height = (OutputLineCount + 1)    * ComponentDefaults.TerminalSpacing + ComponentDefaults.GridSpacing;

        // n selection lines + 1 data input + 2^n data outputs
        Terminals = new Terminal[SelectionLineCount + 1 + OutputLineCount];

        AddTerminalPoints();

        IsHitTestVisible = true;
    }

    public void ComputeOutput()
    {
        // Determine selected output index from selection bits (MSB at S{n-1}, like your Multiplexer)
        int selectedIndex = 0;
        for (int i = 0; i < SelectionLineCount; i++)
        {
            selectedIndex += Terminals![i].Wire!.Value == LogicState.High
                ? (int)Math.Pow(2, SelectionLineCount - i - 1)
                : 0;
        }

        // Input terminal index (immediately after selection lines)
        int inputIndex = SelectionLineCount;

        // Drive all outputs; only the selected one mirrors the input
        for (int k = 0; k < OutputLineCount; k++)
        {
            int outIdx = SelectionLineCount + 1 + k;
            Terminals![outIdx].Wire!.Value = (k == selectedIndex)
                ? Terminals[inputIndex].Wire!.Value
                : LogicState.Low; // change to HighZ/Undefined if your engine supports tri-state
        }
    }

    public override object Clone()
    {
        var clone = new Demultiplexer(this.SelectionLineCount);

        clone.Width = this.Width;
        clone.Height = this.Height;
        clone.Rotation = this.Rotation;
        clone.IsSelected = this.IsSelected;

        for (int i = 0; i < this.Terminals!.Length; i++)
        {
            clone.Terminals![i] = CloneTerminalWithWires(this.Terminals[i], clone.Terminals[i].Position);
        }

        clone.VisualChildren.Clear();
        clone.InvalidateVisual();

        return clone;
    }

    public override void Draw(DrawingContext ctx)
    {
        // Component Body
        ctx.DrawRectangle(ComponentDefaults.GateFillBrush,
            ComponentDefaults.GatePen,
            new Rect(0, 0, Width, Height));

        // Labels: data input 'D' (left), selection 'S' (bottom), data outputs 'Y' (right)
        DrawTerminalsAndLabels(ctx, inputLabel: 'D', outputLabel: 'Y', selectionLabel: 'S');

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

    // Layout: one data input on the left (centered vertically), selection lines at bottom, many outputs on right
    protected void AddTerminalPoints()
    {
        // Helper to snap to nearest grid
        Point SnapToGrid(Point pt)
        {
            double snapX = Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            double snapY = Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            return new Point(snapX, snapY);
        }

        // Selection lines (bottom), spaced like in Multiplexer
        for (int i = 0; i < SelectionLineCount; i++)
        {
            Point pos = new Point((i + 1) * ComponentDefaults.TerminalSpacing, Height + ComponentDefaults.TerminalWireLength);
            Terminals![i] = new Terminal(SnapToGrid(pos), null!);
        }

        // Single data input (left), vertically centered
        {
            var pos = new Point(-ComponentDefaults.TerminalWireLength, Height / 2);
            Terminals![SelectionLineCount] = new Terminal(SnapToGrid(pos), null!);
        }

        // Data outputs (right), stacked from top to bottom
        for (int k = 0; k < OutputLineCount; k++)
        {
            // Keep the same small -5 offset trick as in Multiplexer for visual alignment
            var pos = new Point(Width + ComponentDefaults.TerminalWireLength - 5,
                                ComponentDefaults.TerminalSpacing * (k + 1));
            // Snap only Y like your Mux output terminal did (preserve X offset)
            var snapped = SnapToGrid(pos);
            Terminals![SelectionLineCount + 1 + k] = new Terminal(new Point(pos.X, snapped.Y), null!);
        }
    }

    protected void DrawTerminalsAndLabels(DrawingContext ctx, char inputLabel, char outputLabel, char selectionLabel)
    {
        // Selection lines (bottom)
        for (int i = 0; i < SelectionLineCount; i++)
        {
            ctx.DrawLine(ComponentDefaults.WirePen, Terminals![i].Position, new Point(Terminals[i].Position.X, Height));
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
                Terminals[i].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

            var text = new FormattedText(
                $"{selectionLabel}{SelectionLineCount - i - 1}",
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                ComponentDefaults.LabelTypeface,
                ComponentDefaults.LabelSize,
                ComponentDefaults.LabelBrush
            );
            ctx.DrawText(text, new Point(Terminals[i].Position.X - 7, Height - 18.5));
        }

        // Data input (left)
        {
            int inputIndex = SelectionLineCount;
            ctx.DrawLine(ComponentDefaults.WirePen, Terminals![inputIndex].Position, new Point(0, Terminals[inputIndex].Position.Y));
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
                Terminals[inputIndex].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

            var text = new FormattedText(
                $"{inputLabel}",
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                ComponentDefaults.LabelTypeface,
                ComponentDefaults.LabelSize,
                ComponentDefaults.LabelBrush
            );
            // Slight nudge right of the left edge, vertically centered on the input terminal
            ctx.DrawText(text, new Point(4.5, Terminals[inputIndex].Position.Y - 6));
        }

        // Data outputs (right)
        for (int k = 0; k < OutputLineCount; k++)
        {
            int outIdx = SelectionLineCount + 1 + k;

            ctx.DrawLine(
                ComponentDefaults.WirePen,
                Terminals![outIdx].Position,
                new Point(Terminals[outIdx].Position.X - ComponentDefaults.TerminalWireLength + 5, Terminals[outIdx].Position.Y));

            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
                Terminals[outIdx].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

            var text = new FormattedText(
                $"{outputLabel}{k}",
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                ComponentDefaults.LabelTypeface,
                ComponentDefaults.LabelSize,
                ComponentDefaults.LabelBrush
            );

            // Place label just left inside the body near the right edge
            ctx.DrawText(
                text,
                new Point(Width - 18, Terminals[outIdx].Position.Y - 6)
            );
        }
    }
}
