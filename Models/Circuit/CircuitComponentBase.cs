using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace IRis.Models.Circuit;

/*
 This is kind of tricky to implement
 
 The approach I have in mind is:
    1- Every circuit component has Type + Pos + Rotation + a CustomVisual 
    2- The CustomVisual uses Polymorphism to render different graphics on the basis of the componentType.
    
    3- Upon object creation, a CustomVisual obj is made and added to this Control's VisualChildren
    4- Upon a change in selection status or bounds (X,Y,Width,Height), the Control is redrawn
    
    Other than that, i want these to a have a variable number of inputs
    I also want the ability to rotate it while its being previewed
    
    
*/

// Just put something on the Canvas

// The base CircuitComponent class
public abstract class CircuitComponent : Control
{
    // The 'Visual' object responsible for drawing this component
    protected ComponentVisual _componentVisual;
    
    // Common properties for all components
    // public string ComponentType { get; set; }
    public Point Position { get; set; }
    public double Rotation { get; set; }

    protected int NumInputs;
    
    // The points where this component can connect with other components
    // protected to avoid unintentional modification from other pieces of the program
    protected List<Point> TerminalPoints;


    // Common fields for all components
    public static double DefaultWidth = 120;
    public static double DefaultHeight = 80;

    // For controlling the thickness/colors of lines
    public static double WireStroke = 5;
    public static IImmutableSolidColorBrush WireStrokeBrush = Brushes.Black;
    
    public static double GateStroke = 5;
    public static IImmutableSolidColorBrush GateStrokeBrush = Brushes.Black;

    public static double ConnectionPointRadius = GateStroke + 2;
    public static IImmutableSolidColorBrush ConnectionPointBrush = Brushes.DarkSlateGray;

    public static double TerminalWireLength = 30;

    // The Bubble is 10% of the default width
    public static double BubbleSize = DefaultWidth * 0.1;
    

    
    // For selection
    // NOTE: Make sure to Redraw() when IsSelected changes
    public bool IsSelected { get; set; }

    public CircuitComponent(int numInputs, double widthMult = 1.0, double heightMult = 1.0)
    {
        NumInputs = numInputs;
        
        // Create an empty list of terminal points
        TerminalPoints = new List<Point>();
        
        _componentVisual = new ComponentVisual(this);
        VisualChildren.Add(_componentVisual);
        
        // // Initialize with default values
        Width = DefaultWidth * widthMult;
        Height = DefaultHeight * heightMult;
        
        
        // Add the terminal points
        // NOTE: THIS WAY DOESN"T GET MY CUSTOM IMPLEMENTATION DO SOMETHING ELSE
         AddTerminalPoints();
        
        
       
    }
    // Component-specific drawing
    public virtual void Draw(DrawingContext context)
    {
    }

    // Adds the input and output terminal points
    // Can be overriden for a custom implementation
    protected virtual void AddTerminalPoints()
    {
        // Add terminal points based on number of inputs
        double inputSpacing = Height / (NumInputs + 1);
        for (int i = 1; i <= NumInputs; i++)
        {
            TerminalPoints.Add(new Point(0, inputSpacing * i ));
        }
        
        //Output
        TerminalPoints.Add(new Point(Width, Height / 2));
    }

    // For drawing the terminals
    protected void DrawTerminals(DrawingContext context)
    {
        Pen wirePen = new Pen(WireStrokeBrush, WireStroke);
        
        // Using terminal points to procedurally add terminals to each gate
        // The last terminal point is taken as output
        for (int i = 0; i < TerminalPoints.Count - 1; i++)
        {
            // // Input A (left)
            // context.DrawLine(pen, new Point(-15, 30), new Point(0, 30));
            // context.DrawEllipse(Brushes.Black, null, new Point(-15, 30), terminalRadius, terminalRadius);
            
            var point = TerminalPoints[i];
            context.DrawLine(wirePen, new Point(-TerminalWireLength, point.Y), new Point(0, point.Y));
            context.DrawEllipse(ConnectionPointBrush, null, 
                new Point(point.X - TerminalWireLength, point.Y),
                ConnectionPointRadius, ConnectionPointRadius);
        }
        // For output

        Point outputEndpoint = new Point(TerminalWireLength + TerminalPoints.Last().X, TerminalPoints.Last().Y);
        
        context.DrawLine(wirePen, TerminalPoints.Last(), 
            outputEndpoint);
        
        context.DrawEllipse(ConnectionPointBrush, null, outputEndpoint, ConnectionPointRadius, ConnectionPointRadius);

    }
    
    
    
}
// Contains all the shared drawing code
// AND exposes a virtual method Draw() to add specific drawing code
public class ComponentVisual : Visual
{
    // The component that this class is drawing
    private CircuitComponent _component;

    public ComponentVisual(CircuitComponent component)
    {
        _component = component;
    }

    public override void Render(DrawingContext context)
    {
        _component.Draw(context);
        //context.DrawRectangle(null, new Pen(Brushes.Red, 10), new Rect(0,0,100,100));
    }
}



