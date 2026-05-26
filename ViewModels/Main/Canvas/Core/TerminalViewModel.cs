using System;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects;


namespace IRis.ViewModels.Main.Canvas.Core;


public partial class TerminalViewModel(Terminal model, TerminalType type, bool isOrphan) :
    ObservableObject
{
    private Terminal Model { get; set; } = model;

    public Terminal GetModel() => Model;
    public void SetState(LogicState state) => Model.State = state;
    public LogicState GetState() => Model.State;

    public TerminalType Type = type;
    public bool IsOrphan { get; set; } = isOrphan;

    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;


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
