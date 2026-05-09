using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia;

namespace IRis.Models.Core;

public static class Constants
{
    // DO NOT TOUCH
    public const int GridSpacing = 10;


    // terminals
    public const int TerminalWireLength = 25;
    public const int TerminalBubbleRadius = 7;

    public static readonly Pen TerminalWirePen = new(Brushes.Black, 4);
    public static readonly IBrush TerminalBubbleBrush = Brushes.DarkSlateGray;


    // gates
    public const int NotBubbleRadius = 10;
    public const int OrArcDepth = 6;
    public const int AndArcDepth = 15;

    public static readonly Pen GatePen = new(Brushes.Black, 4);
    public static readonly IBrush GateBrush = Brushes.White;
    public static readonly IBrush NotBubbleBrush = Brushes.White;






    // Common fields for all components
    public const double DefaultHeight = 60;
    public const double DefaultMuxWidth = 80;
    public const double DefaultMuxHeight = 180;

    // For controlling the thickness/colors of lines
    public static Pen WirePen = new Pen(Brushes.Black, 4);
    public static Pen GhostWirePen = new Pen(Brushes.Gray, 4);
    public static Pen InvalidWirePen = new Pen(Brushes.DarkRed, 4);

    // Terminals
    public static double TerminalRadius = 7;
    public static IImmutableSolidColorBrush TerminalBrush = Brushes.DarkSlateGray;
    public static IImmutableSolidColorBrush GhostTerminalBrush = Brushes.LightSlateGray;
    public static IImmutableSolidColorBrush InvalidTerminalBrush = Brushes.DarkRed;

    // Selection
    public static Pen SelectionPen = new Pen(Brushes.DodgerBlue, 2);
    public static SolidColorBrush SelectionBrush = new SolidColorBrush(Colors.DodgerBlue, 0.2);
    
    // For probes/toggles
    public static IImmutableSolidColorBrush TrueBrush = Brushes.ForestGreen;
    public static IImmutableSolidColorBrush FalseBrush = Brushes.DarkRed;
    public static IImmutableSolidColorBrush DontCareBrush = Brushes.Gray;
    
    // For the grid
    public static IBrush GridBrush = new SolidColorBrush(Colors.Black, 0.3);
    public static double GridThickness = 0.5;

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

