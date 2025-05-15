using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace IRis.Models.Circuit;

public class ComponentAnd : CircuitComponent
{
    // NOTE: THIS IS UGLY, MAKE IT LOOK LESS UGLY
    public ComponentAnd(int numInputs) : base(numInputs)
    {
    }
    // {
    //     // Define input and output terminal points (left, right)
    //     // TerminalPoints.Add(new Point(0, 15));   // Input A (left side)
    //     // TerminalPoints.Add(new Point(0, 65));   // Input B (left side)
    //     // TerminalPoints.Add(new Point(100, 40)); // Output (right side)
    //
    // }

    public override void Draw(DrawingContext context)
    {
        // This draws the gates
        Pen gatePen = new Pen(GateStrokeBrush, GateStroke);

        // 1. Create the AND gate shape as a single PathGeometry
        var gatePath = new PathGeometry();
        var figure = new PathFigure
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        double arcLen = Width / 3;

        // Left vertical line
        figure.Segments.Add(new LineSegment { Point = new Point(0, Height) });

        // Bottom horizontal line (left to right)
        figure.Segments.Add(new LineSegment { Point = new Point(Width - arcLen, Height) });

        // Right semicircular arc (bottom to top)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(Width - arcLen, 0),
            Size = new Size(arcLen, Height / 2),
            SweepDirection = SweepDirection.CounterClockwise,

            IsLargeArc = false
        });

        // Top horizontal line (right to left)
        figure.Segments.Add(new LineSegment { Point = new Point(0, 0) });

        gatePath.Figures.Add(figure);

        // 2. Draw the complete gate
        context.DrawGeometry(null, gatePen, gatePath);

        // 3. Draw terminals (lines + circles)
        DrawTerminals(context);
    }
}

public class ComponentNand : CircuitComponent
{
    // NOTE: THIS IS UGLY, MAKE IT LOOK LESS UGLY
    public ComponentNand(int numInputs) : base(numInputs)
    {
    }
    // {
    //     // Define input and output terminal points (left, right)
    //     // TerminalPoints.Add(new Point(0, 15));   // Input A (left side)
    //     // TerminalPoints.Add(new Point(0, 65));   // Input B (left side)
    //     // TerminalPoints.Add(new Point(100, 40)); // Output (right side)
    //
    // }

    public override void Draw(DrawingContext context)
    {
        // This draws the gates
        Pen gatePen = new Pen(GateStrokeBrush, GateStroke);
        
        double dotLen = Width / 10; // Length of the output "bubble"
        double bubbleRadius = dotLen / 2; // Radius of the bubble

        // 1. Create the AND gate shape as a single PathGeometry
        var gatePath = new PathGeometry();
        var figure = new PathFigure
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        double arcLen = Width / 3;

        // Left vertical line
        figure.Segments.Add(new LineSegment { Point = new Point(0, Height) });

        // Bottom horizontal line (left to right)
        figure.Segments.Add(new LineSegment { Point = new Point(Width - arcLen - dotLen, Height) });

        // Right semicircular arc (bottom to top)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(Width - arcLen - dotLen, 0),
            Size = new Size(arcLen, Height / 2),
            SweepDirection = SweepDirection.CounterClockwise,

            IsLargeArc = false
        });

        // Top horizontal line (right to left)
        figure.Segments.Add(new LineSegment { Point = new Point(0, 0) });

        gatePath.Figures.Add(figure);

        // 2. Draw the complete gate
        context.DrawGeometry(null, gatePen, gatePath);
        
        // 3. Draw the output bubble (circle at tip)
        var bubbleCenter = new Point(Width - dotLen / 2, Height / 2);
        context.DrawEllipse(
            Brushes.Transparent, // Fill (none)
            gatePen, // Use same pen as gate
            bubbleCenter,
            bubbleRadius,
            bubbleRadius);

        // 3. Draw terminals (lines + circles)
        DrawTerminals(context);
    }
}


public class ComponentNot : CircuitComponent
{
    // This component always has 1 input and is smaller
    public ComponentNot() : base(1, 0.5)
    {
    }

    public override void Draw(DrawingContext context)
    {
        Pen gatePen = new Pen(GateStrokeBrush, GateStroke);
        double dotLen = Width / 5; // Length of the output "bubble"
        double bubbleRadius = dotLen / 2; // Radius of the bubble

        // 1. Create the triangular NOT gate shape
        var gatePath = new PathGeometry();
        var figure = new PathFigure
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        // Left vertical line down
        figure.Segments.Add(new LineSegment { Point = new Point(0, Height) });

        // Diagonal to tip (right-middle)
        figure.Segments.Add(new LineSegment { Point = new Point(Width - dotLen, Height / 2) });

        // Diagonal back to start
        figure.Segments.Add(new LineSegment { Point = new Point(0, 0) });

        gatePath.Figures.Add(figure);

        // 2. Draw the triangle
        context.DrawGeometry(null, gatePen, gatePath);

        // 3. Draw the output bubble (circle at tip)
        var bubbleCenter = new Point(Width - dotLen / 2, Height / 2);
        context.DrawEllipse(
            Brushes.Transparent, // Fill (none)
            gatePen, // Use same pen as gate
            bubbleCenter,
            bubbleRadius,
            bubbleRadius);

        // 4. Draw terminals (lines + circles)
        DrawTerminals(context);
    }
}

public class ComponentOr : CircuitComponent
{
    // This component always has 1 input and is smaller
    public ComponentOr(int numInputs) : base(numInputs)
    {
    }


    public override void Draw(DrawingContext context)
    {
        Pen gatePen = new Pen(GateStrokeBrush, GateStroke);
        double terminalRadius = 4;

        // 1. Create the OR gate using 3 arcs
        var gatePath = new PathGeometry();
        var figure = new PathFigure
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        // Arc 1: Top curve (right to left)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(0, Height),
            Size = new Size(Width / 6, Height /2),
            SweepDirection = SweepDirection.Clockwise,
            IsLargeArc = false
        });

        // Arc 2: Bottom right curve (left to right)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(Width, Height * 0.5),
            Size = new Size(Width , Height /2),
            SweepDirection = SweepDirection.CounterClockwise,
            IsLargeArc = false
        });

        // Arc 3: Top right curve (right to left)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(0, 0),
            Size = new Size(Width , Height /2),
            SweepDirection = SweepDirection.CounterClockwise,
            IsLargeArc = false
        });

        gatePath.Figures.Add(figure);

        // 2. Draw the complete gate
        context.DrawGeometry(null, gatePen, gatePath);

        // 3. Draw terminals (input left, output right)
        DrawTerminals(context);
    }
}

public class ComponentNor : CircuitComponent
{
    // This component always has 1 input and is smaller
    public ComponentNor(int numInputs) : base(numInputs)
    {
    }


    public override void Draw(DrawingContext context)
    {
        Pen gatePen = new Pen(GateStrokeBrush, GateStroke);
        double terminalRadius = 4;

        // 1. Create the OR gate using 3 arcs
        var gatePath = new PathGeometry();
        var figure = new PathFigure
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        // Arc 1: Top curve (right to left)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(0, Height),
            Size = new Size(Width / 6, Height /2),
            SweepDirection = SweepDirection.Clockwise,
            IsLargeArc = false
        });

        // Arc 2: Bottom right curve (left to right)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(Width, Height * 0.5),
            Size = new Size(Width , Height /2),
            SweepDirection = SweepDirection.CounterClockwise,
            IsLargeArc = false
        });

        // Arc 3: Top right curve (right to left)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(0, 0),
            Size = new Size(Width , Height /2),
            SweepDirection = SweepDirection.CounterClockwise,
            IsLargeArc = false
        });

        gatePath.Figures.Add(figure);

        // 2. Draw the complete gate
        context.DrawGeometry(null, gatePen, gatePath);

        // 3. Draw terminals (input left, output right)
        DrawTerminals(context);
    }
}

