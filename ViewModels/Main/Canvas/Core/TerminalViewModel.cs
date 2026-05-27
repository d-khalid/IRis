using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;
using IRis.Models.Main.Canvas.Core;
using Newtonsoft.Json;


namespace IRis.ViewModels.Main.Canvas.Core;


public partial class TerminalViewModel : ObservableObject
{
    private Terminal Model { get; } = new Terminal();
    public Terminal GetModel() => Model;

    public TerminalType Type;
    [JsonIgnore] public bool IsOrphan = false;

    [ObservableProperty] [property: JsonIgnore] private double _x;
    [ObservableProperty] [property: JsonIgnore] private double _y;
    [ObservableProperty] [property: JsonIgnore] private string _color = "DarkGray";


    public TerminalViewModel()
    {
        Model.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(Terminal.State))
                UpdateColor();
        };
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
