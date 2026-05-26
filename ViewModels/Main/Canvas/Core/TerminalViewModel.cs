using System;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;
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
        var sel = Selection.GetInstance();
        var prev = Preview.GetInstance();

        sel.DitchPartial();
        if (prev.HasNewWire())
            prev.EndWireAt(this);
        else
            prev.StartWireAt(this);
    }


    public void PointerEntered()
    {
        var sel = Selection.GetInstance();
        sel.HidePartial();
    }


    public void PointerExited()
    {
        var sel = Selection.GetInstance();
        sel.ShowPartial();
    }
}
