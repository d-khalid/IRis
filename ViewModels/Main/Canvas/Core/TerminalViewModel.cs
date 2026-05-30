using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services;
using IRis.Services.Singleton;
using IRis.Models.Core;
using Newtonsoft.Json;
using Avalonia.Input;


namespace IRis.ViewModels.Main.Canvas.Core;


public partial class TerminalViewModel : ObservableObject
{
    private Terminal Model { get; } = new Terminal();
    public Terminal GetModel() => Model;

    public TerminalType Type;
    [JsonIgnore] public bool IsOrphan = false;

    [ObservableProperty] [property: JsonIgnore] private double _x = 0;
    [ObservableProperty] [property: JsonIgnore] private double _y = 0;
    [ObservableProperty] [property: JsonIgnore] private string _color = "DarkGray";
    [ObservableProperty] [property: JsonIgnore] private Cursor _cursor = new(StandardCursorType.Arrow);


    public TerminalViewModel()
    {
        Model.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(Terminal.State)) UpdateColor();
        };
    }


    private void UpdateColor()
    {
        if (!AppState.GetInstance().TerminalColorChangeAllowed)
        {
            Color = "DarkGray";
            return;
        }

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
        if (!Selection.GetInstance().IsEmpty()) 
            Selection.GetInstance().UnHighlightAll();

        HoverEffectService.Stop();

        if (Preview.GetInstance().HasNewWire())
            Preview.GetInstance().EndWireAt(this);
        else
            Preview.GetInstance().StartWireAt(this);
    }


    public void PointerEntered()
    {
        Cursor = new(StandardCursorType.Cross);
        HoverEffectService.Hide();
    }


    public void PointerExited()
    {
        Cursor = new(StandardCursorType.Arrow);
        HoverEffectService.Show();
    }
}
