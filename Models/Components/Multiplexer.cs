using System;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;

namespace IRis.Models.Components;

public class Multiplexer : Component, IOutputProvider
{
 
    public Multiplexer(int selectionLineCount, double width = ComponentDefaults.DefaultMuxWidth,
        double height = ComponentDefaults.DefaultMuxHeight)
        : base(width, height)
    {
        SelectionLineCount = selectionLineCount;
        InputLineCount = (int)Math.Pow(2, SelectionLineCount);
        
        Width = (SelectionLineCount + 1) * ComponentDefaults.TerminalSpacing;
        Height = (InputLineCount + 1) * ComponentDefaults.TerminalSpacing + ComponentDefaults.GridSpacing;

        
        // n selection lines map to 2^n lines and 1 output
        Terminals = new Terminal[SelectionLineCount + (int)Math.Pow(2, SelectionLineCount) + 1];

        AddTerminalPoints();

        IsHitTestVisible = true;
    }

    public void ComputeOutput()
    {
        // Find the selected line
        int selectedIndex = 0;
        for (int i = 0; i < SelectionLineCount; i++)
        {
            if (Terminals![i].Wire!.Value == LogicState.High)
            {
                // Top is MSB, bottom is LSB
                selectedIndex |= (1 << (SelectionLineCount - i - 1));
            }
        }

        Terminals![^1].Wire!.Value = Terminals[SelectionLineCount + selectedIndex].Wire!.Value;
    }

    public override void Draw(DrawingContext ctx)
    {
        
        // Component Body
        ctx.DrawRectangle(ComponentDefaults.GateFillBrush,
            ComponentDefaults.GatePen, 
            new Rect(0,0, Width, Height));
        
        DrawTerminalsAndLabels(ctx, 'D', 'S');

        
        base.Draw(ctx);
    }

    public override void DrawSelection(DrawingContext ctx)
    {
        double expandX = ComponentDefaults.TerminalWireLength + ComponentDefaults.TerminalRadius ;
        double expandY = ComponentDefaults.TerminalRadius ;
        // Subtle fill
        ctx.DrawRectangle(
            ComponentDefaults.SelectionBrush, 
            ComponentDefaults.SelectionPen, 
            new Rect(
                -expandX,
                -expandY,
                Bounds.Width + 2 * expandX ,
                Bounds.Height + 2 * expandY)
        );
    }

    // Inputs are on the left, output on right, selection lines at bottom
    protected void AddTerminalPoints()
    {
        // Helper to snap to nearest multiple of 10
        Point SnapToGrid(Point pt)
        {
            double snapX = Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            double snapY = Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            return new Point(snapX, snapY);
        }
        
        // For selection lines
        for (int i = 0; i < SelectionLineCount; i++)
        {
            Point pos = new Point((i + 1) * ComponentDefaults.TerminalSpacing, Height + ComponentDefaults.TerminalWireLength);
            Terminals![i] = new Terminal(SnapToGrid(pos), null!);
        }
        
        // For input lines
        for (int i = SelectionLineCount; i < InputLineCount + SelectionLineCount; i++)
        {
            var pos = new Point(-ComponentDefaults.TerminalWireLength, ComponentDefaults.TerminalSpacing * (i - SelectionLineCount + 1));
            Terminals![i] = new Terminal(SnapToGrid(pos), null!);
        }

        // For outputoutputPos
        // Fix: do not snap the X value of the outputPos
        Point outputPos = new Point(Width + ComponentDefaults.TerminalWireLength - 5, Height/2);
        Terminals![^1] = new Terminal(new Point(outputPos.X, SnapToGrid(outputPos).Y), null!);
    }

    protected void DrawTerminalsAndLabels(DrawingContext ctx, char inputLabel, char selectionLabel)
    {
        // For selection lines
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
            ctx.DrawText(
                text,
                new Point(Terminals[i].Position.X - 7, Height - 18.5)
                );

        }

        // For input lines
        for (int i = SelectionLineCount; i < InputLineCount + SelectionLineCount; i++)
        {
            ctx.DrawLine(ComponentDefaults.WirePen, Terminals![i].Position, new Point(0, Terminals[i].Position.Y));
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
                Terminals[i].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

            var text = new FormattedText(
                $"{inputLabel}{i - SelectionLineCount}",
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                ComponentDefaults.LabelTypeface,
                ComponentDefaults.LabelSize,
                ComponentDefaults.LabelBrush
            );
            ctx.DrawText(
                text,
                new Point(4.5, Terminals[i].Position.Y - 6)
            );

        }
        // For output
        ctx.DrawLine(ComponentDefaults.WirePen, Terminals![^1].Position,
            new Point(Terminals[^1].Position.X - ComponentDefaults.TerminalWireLength + 5, Terminals[^1].Position.Y));
        ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
            Terminals[^1].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);
    }
    
    
}