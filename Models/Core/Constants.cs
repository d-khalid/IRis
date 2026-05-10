using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia;

namespace IRis.Models.Core;

public static class Constants
{
    // DO NOT TOUCH THIS ONE!
    public const int GridSpacing = 10;

    public const int TerminalWireLength = 25;
    public const int TerminalBubbleRadius = 7;
    public const int NotBubbleRadius = 10;
    public const int OrArcDepth = 6;
    public const int AndArcDepth = 15;
    public const int WireWidth = 4;
    public const double CanvasGridThickness = 0.5;

    public static readonly Pen GatePen = new(Brushes.Black, 4);
    public static readonly IBrush GateBrush = Brushes.White;
    public static readonly IBrush NotBubbleBrush = Brushes.White;

    public static readonly Pen WirePen = new(Brushes.Black, WireWidth);
    public static readonly Pen GhostWirePen = new(Brushes.Gray, WireWidth);
    public static readonly Pen InvalidWirePen = new(Brushes.DarkRed, WireWidth);

    public static readonly Pen TerminalWirePen = new(Brushes.Black, 4);
    public static readonly Pen GhostTerminalWirePen = new(Brushes.Gray, 4);
    public static readonly Pen InvalidTerminalWirePen = new(Brushes.DarkRed, 4);
    public static readonly IBrush TerminalBubbleBrush = Brushes.DarkSlateGray;
    public static readonly IBrush GhostTerminalBubbleBrush = Brushes.LightSlateGray;
    public static readonly IBrush InvalidTerminalBubbleBrush = Brushes.DarkRed;

    public static readonly Pen LogicProbePen = new(Brushes.Black, 4);
    public static readonly IBrush LogicProbeBrush = Brushes.White;
    public static readonly Pen LogicTogglePen = new(Brushes.Black, 4);
    public static readonly IBrush LogicToggleBrush = Brushes.White;

    public static readonly ImmutableSolidColorBrush TrueStateBrush = new(Colors.ForestGreen);
    public static readonly ImmutableSolidColorBrush FalseStateBrush = new(Colors.DarkRed);
    public static readonly ImmutableSolidColorBrush UnknownStateBrush = new(Colors.Gray);

    public static readonly IBrush CanvasGridBrush = new SolidColorBrush(Colors.Black, 0.3);

    public const int AndGateDefaultNumInputs = 2;
    public const int OrGateDefaultNumInputs = 2;
    public const int NandGateDefaultNumInputs = 2;
    public const int NorGateDefaultNumInputs = 2;
    public const int XorGateDefaultNumInputs = 2;
    public const int XnorGateDefaultNumInputs = 2;

    public static readonly BoxSize AndGateSize = new(width: 60, height: 60);
    public static readonly BoxSize OrGateSize = new(width: 60, height: 60);
    public static readonly BoxSize NandGateSize = new(width: 60, height: 60);
    public static readonly BoxSize NorGateSize = new(width: 60, height: 60);
    public static readonly BoxSize XorGateSize = new(width: 60, height: 60);
    public static readonly BoxSize XnorGateSize = new(width: 60, height: 60);
    public static readonly BoxSize NotGateSize = new(width: 45, height: 45);

    public static readonly BoxSize LogicProbeSize = new(width: 35, height: 35);
    public static readonly BoxSize LogicToggleSize = new(width: 35, height: 35);

    public const int DrawingBigTextSize = 20;
    public static readonly Typeface DrawingBigTextTypeFace = new(
        fontFamily: "Arial", weight: FontWeight.Bold
    );




    // Common fields for all components
    public const double DefaultHeight = 60;
    public const double DefaultMuxWidth = 80;
    public const double DefaultMuxHeight = 180;

    // Terminals
    public static double TerminalRadius = 7;

    // Selection
    public static Pen SelectionPen = new Pen(Brushes.DodgerBlue, 2);
    public static SolidColorBrush SelectionBrush = new SolidColorBrush(Colors.DodgerBlue, 0.2);
    
    // For probes/toggles
    
    // For the grid

    // For terminals
    public static double TerminalSnappingRange = 15;
    public const double TerminalSpacing = 20;

    // For labels
    public static IBrush LabelBrush = new ImmutableSolidColorBrush(Color.FromRgb(40, 40, 40));
    public static double LabelSize = 12;
    public static Typeface LabelTypeface = new Typeface(fontFamily: "Source Code Pro", weight: FontWeight.SemiBold); 

    // For NOT-derived Gates
    public static double BubbleRadius = 80 / 15;

    // Higher makes the arc on OR gates steeper
    public static double OrArcFactor = 6;
    
    // High brings the 2nd arc closer to the main arc on the gate
    public static double XorArcDistFactor = 3;
}

