using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace IRis.Models.Core;

public static class ComponentDefaults
{
    // Common fields for all components
    public const double DefaultWidth = 100;
    public const double DefaultHeight = 80;

    // For controlling the thickness/colors of lines
    public static Pen WirePen = new Pen(Brushes.Black, 5);
    
    public static Pen GatePen = new Pen(Brushes.Black, 5);
    
    public static IImmutableSolidColorBrush GateFillBrush = Brushes.White;


    public static double TerminalRadius = 7;
    public static IImmutableSolidColorBrush TerminalBrush = Brushes.DarkSlateGray;

    public static Pen SelectionPen = new Pen(Brushes.DodgerBlue, 2);
    public static SolidColorBrush SelectionBrush = new SolidColorBrush(Colors.DodgerBlue, 0.2);
    
    // For probes/toggles
    public static IImmutableSolidColorBrush TrueBrush = Brushes.ForestGreen;
    public static IImmutableSolidColorBrush FalseBrush = Brushes.DarkRed;
    public static IImmutableSolidColorBrush DontCareBrush = Brushes.Gray;



    

    public static double TerminalWireLength = 30;

    // For NOT-derived Gates
    public static double BubbleRadius = DefaultWidth / 15;

    // Higher makes the arc on OR gates steeper
    public static double OrArcFactor = 6;
    
    // High brings the 2nd arc closer to the main arc on the gate
    public static double XorArcDistFactor = 3;
    
    // For the grid
    public const double GridSpacing = 20; // pixels between grid lines
    public static IBrush GridBrush = new SolidColorBrush(Colors.Black, 0.3);
    public static double GridThickness = 0.5;


}