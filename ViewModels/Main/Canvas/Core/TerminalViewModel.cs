using System;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects;


namespace IRis.ViewModels.Main.Canvas.Core;


public partial class TerminalViewModel : ObservableObject
{
    private Terminal Model { get; set; } = null!;
    public Terminal GetModel() => Model;

    public TerminalType Type;
    public bool IsOrphan { get; set; }

    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;
    [ObservableProperty] private string _color = "DarkGray";


    public TerminalViewModel(Terminal model, TerminalType type, bool isOrphan)
    {
        Model = model;
        Type = type;
        IsOrphan = isOrphan;

        if (Model is not null)
        {
            Model.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName is nameof(Terminal.State))
                    UpdateColor();
            };
        }
    }


    private void UpdateColor()
    {
        Color = Model.State switch
        {
            LogicState.High => "DarkGreen",
            LogicState.Low => "DarkRed",
            LogicState.Unknown => "DarkGray",
            _ => "DarkGray"
        };
    }


    public void PointerPressed()
    {
        var sel = Selection.GetInstance();
        var prev = Preview.GetInstance();

        sel.DitchPartial();
        sel.Ditch();

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
