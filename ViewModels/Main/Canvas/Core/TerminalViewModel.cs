using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services;
using IRis.Services.Singleton;
using IRis.Models.Core;
using Newtonsoft.Json;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia;
using Avalonia.Controls;


namespace IRis.ViewModels.Main.Canvas.Core;


public partial class TerminalViewModel : ObservableObject
{
    private Terminal Model { get; } = new Terminal();
    public Terminal GetModel() => Model;

    public TerminalType Type;
    [JsonIgnore] public bool IsOrphan = false;

    [ObservableProperty][property: JsonIgnore] private double _x = 0;
    [ObservableProperty][property: JsonIgnore] private double _y = 0;

    [ObservableProperty]
    [property: JsonIgnore]
    private IBrush _color = null!;

    [ObservableProperty]
    [property: JsonIgnore]
    private Cursor _cursor = new(StandardCursorType.Arrow);


    public TerminalViewModel()
    {
        Model.PropertyChanged += (_, e) =>
        {
            if (AppState.Get().TerminalColorChangeAllowed && e.PropertyName is nameof(Terminal.State))
                UpdateColor();
        };

        AppState.Get().PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(AppState.TerminalColorChangeAllowed))
            {
                UpdateColor();
            }
        };

        UpdateColor();
    }


    private void UpdateColor()
    {
        string resource;

        if (AppState.Get().TerminalColorChangeAllowed)
        {
            resource = Model.State switch
            {
                LogicState.High => "HighStateBrush",
                LogicState.Low => "LowStateBrush",
                LogicState.Unknown => "UnknownStateBrush",
                _ => "UnknownStateBrush"
            };
        }

        else
        {
            resource = "UnknownStateBrush";
        }

        Application.Current!.TryGetResource(resource, AppState.Get().Theme, out var res);
        Color = (IBrush)res!;
    }


    public void PointerPressed()
    {
        if (!Selection.Get().IsEmpty())
            Selection.Get().UnHighlightAll();

        HoverEffectService.Stop();

        if (WirePreview.Get().IsEmpty())
            WirePreview.Get().StartAt(this);
        else
            WirePreview.Get().EndAt(this);
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
