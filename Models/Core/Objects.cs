// default libs
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.Models.Core;


public enum ComponentOrientation
{
    Right = 0,
    Down = 90,
    Left = 180,
    Up = 270
}



// SUGGESTION: This struct mostly complicates the XAML, remove it kindly?
// I'm keeping it so I dont need to change a billion constructors

// public struct BoxSize(int width, int height)
// {
//     public int Width = width;
//     public int Height = height;
// }

public partial class BoxSize(int width, int height) : ObservableObject
{
    [ObservableProperty]
    private int _width = width;
    
    [ObservableProperty]
    private int _height= height;
}


public class Terminal(Point position, bool isOutputProvider)
{
    public Point Position { get; set; }= position;
    public readonly bool isOutputProvider = isOutputProvider;
    public LogicState State = LogicState.Unknown;
}


public enum LogicState
{
    High = 1,
    Low = 0,
    Unknown = -1
}

