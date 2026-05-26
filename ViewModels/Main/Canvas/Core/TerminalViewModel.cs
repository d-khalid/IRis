using System;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects;


namespace IRis.ViewModels.Main.Canvas.Core;


public partial class TerminalViewModel(TerminalType type, ComponentViewModel? parent) : ObservableObject
{
    public Terminal Model { get; } = new(type);
    public bool IsOrphan { get; set; } = parent == null;

    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;


    public TerminalType FetchType()
    {
        return Model.Type;
    }


    public void PointerPressed()
    {
        Console.WriteLine("Terminal Pointer Press");
    }
}
