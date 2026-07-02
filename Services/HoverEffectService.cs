using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

namespace IRis.Services;

public class HoverEffectService(AppState appState)
{
    private readonly AppState _appState = appState;
    private CircuitObjectViewModel? Object { get; set; } = null;

    public void On(CircuitObjectViewModel co)
    {
        Object = co;

        if (!_appState.EditingAllowed)
            return;

        if (co is ComponentViewModel c)
            c.SelectionOpacity = 0.5;
        else if (co is WireViewModel w)
            w.SelectionOpacity = 0.2;
    }

    public void Stop()
    {
        if (Object is not null)
        {
            Object.SelectionOpacity = 0.0;
            Object = null;
        }
    }

    public void Hide()
    {
        if (Object is not null)
        {
            Object.SelectionOpacity = 0.0;
        }
    }

    public void Show()
    {
        if (Object is not null)
        {
            Object.SelectionOpacity = 0.5;
        }
    }

    public bool IsRunning() => Object is not null;

    public bool HasToggle() => Object is ToggleViewModel;

    public CircuitObjectViewModel? GetObject() => Object;
}
