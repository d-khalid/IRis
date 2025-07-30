using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;
using IRis.Models;
using IRis.Services;

namespace IRis.Models.Components;

public class CustomComponent : Component, IOutputProvider
{
    protected string ComponentName;
    protected int InputCount;
    protected int OutputCount;
    protected List<CircuitFormulaConversionService.CircuitFormula> OutputFormulas;
    
    public CustomComponent(string name, int inputCount = 2, int outputCount = 1, 
        List<CircuitFormulaConversionService.CircuitFormula>? outputFormulas = null,
        double width = ComponentDefaults.DefaultMuxWidth,
        double height = ComponentDefaults.DefaultMuxHeight)
        : base(width, height)
    {
        Console.WriteLine("Creating Custom Component");
        ComponentName = name;
        InputCount = inputCount;
        OutputCount = outputCount;
        OutputFormulas = outputFormulas ?? [];
        
        // Calculate dimensions based on input/output count
        int maxTerminals = Math.Max(InputCount, OutputCount);
        // maxTerminals = (maxTerminals % 2 == 1) ? maxTerminals + 1 : maxTerminals;
        Height = maxTerminals * ComponentDefaults.TerminalSpacing + ComponentDefaults.GridSpacing;
        Height = (Height % 20 == 0) ? Height : Height + 10;
        Width = ComponentName.Length * 10;
        Console.WriteLine("Maxterminals: " + maxTerminals);
        Console.WriteLine("Height: " + Height + ", Width: " + Width + ", Name: " + ComponentName.Length);

        // Total terminals = inputs + outputs
        Terminals = new Terminal[InputCount + OutputCount];

        AddTerminalPoints();

        IsHitTestVisible = true;
    }

    public void ComputeOutput()
    {
        // Compute each output based on its formula
        for (int outputIndex = 0; outputIndex < OutputCount; outputIndex++)
        {
            var result = EvaluateFormula(OutputFormulas[outputIndex].Formula);
            Terminals![InputCount + outputIndex].Wire!.Value = result;
        }
    }

    private LogicState EvaluateFormula(string formula)
    {
        try
        {
            // Replace input variables with actual values
            string processedFormula = formula;
            
            // Replace Input_X format variables
            for (int i = 1; i <= InputCount; i++)
            {
                string inputVar = $"Input_{i}";
                bool inputValue = i <= InputCount && Terminals![i - 1].Wire?.Value == LogicState.High;
                processedFormula = processedFormula.Replace(inputVar, inputValue ? "1" : "0");
            }
            
            // Also handle A, B, C... format variables
            for (int i = 0; i < InputCount; i++)
            {
                char inputVar = (char)('A' + i);
                bool inputValue = Terminals![i].Wire?.Value == LogicState.High;
                processedFormula = processedFormula.Replace(inputVar.ToString(), inputValue ? "1" : "0");
            }
            
            return EvaluateBooleanExpression(processedFormula) ? LogicState.High : LogicState.Low;
        }
        catch
        {
            return LogicState.Low; // Default to low on error
        }
    }

    private bool EvaluateBooleanExpression(string expression)
    {
        expression = expression.Replace(" ", "");
        return ParseExpression(expression, 0).result;
    }

    private (bool result, int nextIndex) ParseExpression(string expr, int startIndex)
    {
        return ParseOrExpression(expr, startIndex);
    }

    private (bool result, int nextIndex) ParseOrExpression(string expr, int startIndex)
    {
        var (left, nextIndex) = ParseAndExpression(expr, startIndex);
        
        while (nextIndex < expr.Length && expr[nextIndex] == '|')
        {
            var (right, newIndex) = ParseAndExpression(expr, nextIndex + 1);
            left = left || right;
            nextIndex = newIndex;
        }
        
        return (left, nextIndex);
    }

    private (bool result, int nextIndex) ParseAndExpression(string expr, int startIndex)
    {
        var (left, nextIndex) = ParseNotExpression(expr, startIndex);
        
        while (nextIndex < expr.Length && expr[nextIndex] == '&')
        {
            var (right, newIndex) = ParseNotExpression(expr, nextIndex + 1);
            left = left && right;
            nextIndex = newIndex;
        }
        
        return (left, nextIndex);
    }

    private (bool result, int nextIndex) ParseNotExpression(string expr, int startIndex)
    {
        if (startIndex >= expr.Length)
            return (false, startIndex);

        if (expr[startIndex] == '!')
        {
            var (result, nextIndex) = ParseNotExpression(expr, startIndex + 1);
            return (!result, nextIndex);
        }
        
        return ParsePrimaryExpression(expr, startIndex);
    }

    private (bool result, int nextIndex) ParsePrimaryExpression(string expr, int startIndex)
    {
        if (startIndex >= expr.Length)
            return (false, startIndex);

        if (expr[startIndex] == '(')
        {
            var (result, nextIndex) = ParseExpression(expr, startIndex + 1);
            
            // Skip the closing parenthesis
            if (nextIndex < expr.Length && expr[nextIndex] == ')')
                nextIndex++;
            
            return (result, nextIndex);
        }
        
        if (expr[startIndex] == '1')
            return (true, startIndex + 1);
        
        if (expr[startIndex] == '0')
            return (false, startIndex + 1);
        
        // Handle variables (though they should have been replaced by now)
        if (char.IsLetter(expr[startIndex]))
        {
            // Skip the variable (it should have been replaced)
            return (false, startIndex + 1);
        }
        
        return (false, startIndex + 1);
    }

    public override void Draw(DrawingContext ctx)
    {
        // Component Body
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

    protected void AddTerminalPoints()
    {
        // Snap to multiples of Grid Spacing
        Point SnapToGrid(Point pt)
        {
            double snapX = Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            double snapY = Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            return new Point(snapX, snapY);
        }
        double SnapSpaceToGrid (double space)
        {
            return Math.Round(space / ComponentDefaults.GridSpacing) *  ComponentDefaults.GridSpacing;
        }

        // Add input terminals on the left side
        double inputSpacing = ComponentDefaults.TerminalSpacing; //Height / (InputCount + 1);
        for (int i = 0; i < InputCount; i++)
        {
            Point pos = new Point(-ComponentDefaults.TerminalWireLength, inputSpacing * (i + 1));
            Terminals![i] = new Terminal(SnapToGrid(pos), null!);
        }

        // Add output terminals on the right side
        double outputSpacing = SnapSpaceToGrid(Height / (OutputCount + 1));  //Height / (OutputCount + 1);
        for (int i = 0; i < OutputCount; i++)
        {
            Point pos = new Point(Width + ComponentDefaults.TerminalWireLength, outputSpacing * (i + 1));
            Terminals![InputCount + i] = new Terminal(SnapToGrid(pos), null!);
        }
    }

    protected void DrawTerminalsAndLabels(DrawingContext ctx)
    {
        // Draw input terminals and labels
        for (int i = 0; i < InputCount; i++)
        {
            // Draw connection line
            ctx.DrawLine(ComponentDefaults.WirePen, 
                Terminals![i].Position, 
                new Point(0, Terminals[i].Position.Y));
            
            // Draw terminal circle
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null, 
                Terminals[i].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);
            
            // Draw input label
            char inputLabel = (char)('A' + i);
            var text = new FormattedText(
                inputLabel.ToString(),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                ComponentDefaults.LabelTypeface,
                ComponentDefaults.LabelSize, 
                ComponentDefaults.LabelBrush
            );
            ctx.DrawText(text, new Point(4.5, Terminals[i].Position.Y - 6));
        }
        
        // Draw output terminals and labels
        for (int i = 0; i < OutputCount; i++)
        {
            int terminalIndex = InputCount + i;
            
            // Draw connection line
            ctx.DrawLine(ComponentDefaults.WirePen, 
                Terminals![terminalIndex].Position,
                new Point(Width, Terminals[terminalIndex].Position.Y));
            
            // Draw terminal circle
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null, 
                Terminals[terminalIndex].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);
            
            // Draw output label (Y for single output, Y0, Y1, etc. for multiple)
            string outputLabel = OutputCount == 1 ? "Y" : $"Y{i}";
            var text = new FormattedText(
                outputLabel,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                ComponentDefaults.LabelTypeface,
                ComponentDefaults.LabelSize, 
                ComponentDefaults.LabelBrush
            );
            ctx.DrawText(text, new Point(Width - 15, Terminals[terminalIndex].Position.Y - 6));
        }
        
        // Draw formula text in the center of the component (simplified for display)
        if (OutputCount == 1 && OutputFormulas.Count > 0)
        {
            string displayText = ComponentName;
            
            var formulaText = new FormattedText(
                displayText,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                ComponentDefaults.LabelTypeface,
                ComponentDefaults.LabelSize,
                ComponentDefaults.LabelBrush
            );
            
            double textX = (Width - formulaText.Width) / 2;
            double textY = (Height - formulaText.Height) / 2;
            ctx.DrawText(formulaText, new Point(textX, textY));
        }
    }
}