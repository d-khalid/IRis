using Avalonia.Collections;
using IRis.ViewModels.Main.Canvas;
using IRis.ViewModels.Main.Canvas.CircuitObjects;

namespace IRis.Services.Singleton;

public partial class Selection : SingletonCollection<Selection>
{
    public void Highlight(CircuitObjectViewModel co)
    {
        Objects.Add(co);
        if (co is ComponentViewModel c)
            c.SelectionOpacity = 1;
        else if (co is WireViewModel w)
            w.SelectionOpacity = 0.4;
        co.IsSelected = true;
    }

    public void UnHighlight(CircuitObjectViewModel co)
    {
        Objects.Remove(co);
        co.SelectionOpacity = 0.0;
        co.IsSelected = false;
    }

    public void Highlight(AvaloniaList<CircuitObjectViewModel> collection)
    {
        Objects.AddRange(collection);
        foreach (var co in collection)
        {
            if (co is ComponentViewModel c)
                c.SelectionOpacity = 1;
            else if (co is WireViewModel w)
                w.SelectionOpacity = 0.4;
            co.IsSelected = true;
        }
    }

    public void UnHighlight(AvaloniaList<CircuitObjectViewModel> collection)
    {
        Objects.RemoveAll(collection);
        foreach (var co in collection)
        {
            co.SelectionOpacity = 0.0;
            co.IsSelected = false;
        }
    }

    public void UnHighlightAll()
    {
        foreach (var co in Objects)
        {
            co.SelectionOpacity = 0.0;
            co.IsSelected = false;
        }

        Objects.Clear();
    }

    public bool IsEmpty() => Objects.Count == 0;
}
