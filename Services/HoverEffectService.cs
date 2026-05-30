using IRis.ViewModels.Main.Canvas;
using IRis.ViewModels.Main.Canvas.CircuitObjects;


namespace IRis.Services;


public static class HoverEffectService
{
    private static CircuitObjectViewModel? Object { get; set; } = null;


    public static void On(CircuitObjectViewModel co)
    {
        Object = co;
        if (co is ComponentViewModel c) c.SelectionOpacity = 0.5;
        else if (co is WireViewModel w) w.SelectionOpacity = 0.2;
    }


    public static void Stop()
    {
        if (Object is not null)
        {
            Object.SelectionOpacity = 0.0;
            Object = null;
        }
    }


    public static void Hide()
    {
        if (Object is not null)
        {
            Object.SelectionOpacity = 0.0;
        }
    }


    public static void Show()
    {
        if (Object is not null)
        {
            Object.SelectionOpacity = 0.5;
        }
    }


    public static bool IsRunning() => Object is not null;
}
