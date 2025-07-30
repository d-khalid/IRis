using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;

namespace IRis.Models.Components;

public class CustomComponent : Component, IOutputProvider
{
    protected int InputCount;
    protected int OutputCount;
    protected List<string> OutputFormulas;
    
    public CustomComponent(int inputCount = 2, int outputCount = 1, 
        List<string>? outputFormulas = null,
        double width = ComponentDefaults.DefaultMuxWidth,
        double height = ComponentDefaults.DefaultMuxHeight)
        : base(width, height)
    {
        InputCount = inputCount;
        OutputCount = outputCount;
        OutputFormulas = outputFormulas ?? new List<string>();
        
        // Ensure we have formulas for all outputs (default to "A" for single input, "A&B" for multiple)
        while (OutputFormulas.Count < OutputCount)
        {
            if (InputCount == 1)
                OutputFormulas.Add("A");
            else
                OutputFormulas.Add("A&B"); // Default AND gate behavior
        }
        
        // Calculate dimensions based on input/output count
        int maxTerminals = Math.Max(InputCount, OutputCount);
        Width = 80;
        Height = maxTerminals * ComponentDefaults.TerminalSpacing + ComponentDefaults.GridSpacing;

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
            var result = EvaluateFormula(OutputFormulas[outputIndex]);
            Terminals![InputCount + outputIndex].Wire!.Value = result;
        }
    }

    private LogicState EvaluateFormula(string formula)
    {
        // Simple formula evaluator for boolean expressions
        // Supports: A, B, C... for inputs, &(AND), |(OR), !(NOT), parentheses
        
        try
        {
            // Replace input variables with actual values
            string processedFormula = formula.ToUpper();
            
            for (int i = 0; i < InputCount; i++)
            {
                char inputVar = (char)('A' + i);
                bool inputValue = Terminals![i].Wire?.Value == LogicState.High;
                processedFormula = processedFormula.Replace(inputVar.ToString(), inputValue ? "1" : "0");
            }
            
            // Simple evaluation (you might want to use a proper expression parser)
            return EvaluateSimpleExpression(processedFormula) ? LogicState.High : LogicState.Low;
        }
        catch
        {
            return LogicState.Low; // Default to low on error
        }
    }

    private bool EvaluateSimpleExpression(string expression)
    {
        // Very basic expression evaluator - you might want to implement a proper parser
        // For now, handle simple cases like "1&0", "1|0", "!1", etc.
        
        expression = expression.Replace(" ", "");
        
        // Handle NOT operations first
        while (expression.Contains("!"))
        {
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '!')
                {
                    if (i + 1 < expression.Length)
                    {
                        char nextChar = expression[i + 1];
                        if (nextChar == '1')
                            expression = expression.Substring(0, i) + "0" + expression.Substring(i + 2);
                        else if (nextChar == '0')
                            expression = expression.Substring(0, i) + "1" + expression.Substring(i + 2);
                    }
                    break;
                }
            }
        }
        
        // Handle AND operations
        while (expression.Contains("&"))
        {
            for (int i = 1; i < expression.Length - 1; i++)
            {
                if (expression[i] == '&')
                {
                    char left = expression[i - 1];
                    char right = expression[i + 1];
                    
                    if (char.IsDigit(left) && char.IsDigit(right))
                    {
                        bool result = (left == '1') && (right == '1');
                        expression = expression.Substring(0, i - 1) + (result ? "1" : "0") + expression.Substring(i + 2);
                        break;
                    }
                }
            }
        }
        
        // Handle OR operations
        while (expression.Contains("|"))
        {
            for (int i = 1; i < expression.Length - 1; i++)
            {
                if (expression[i] == '|')
                {
                    char left = expression[i - 1];
                    char right = expression[i + 1];
                    
                    if (char.IsDigit(left) && char.IsDigit(right))
                    {
                        bool result = (left == '1') || (right == '1');
                        expression = expression.Substring(0, i - 1) + (result ? "1" : "0") + expression.Substring(i + 2);
                        break;
                    }
                }
            }
        }
        
        return expression.Contains("1");
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
        Point SnapToGrid(Point pt)
        {
            return pt;
        }
        
        // Add input terminals on the left side
        double inputSpacing = Height / (InputCount + 1);
        for (int i = 0; i < InputCount; i++)
        {
            Point pos = new Point(-ComponentDefaults.TerminalWireLength, inputSpacing * (i + 1));
            Terminals![i] = new Terminal(SnapToGrid(pos), null!);
        }
        
        // Add output terminals on the right side
        double outputSpacing = Height / (OutputCount + 1);
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
        
        // Draw formula text in the center of the component
        if (OutputCount == 1 && OutputFormulas.Count > 0)
        {
            var formulaText = new FormattedText(
                OutputFormulas[0],
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