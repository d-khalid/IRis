using Avalonia.Controls;
using Avalonia.Input;
using Avalonia;
using IRis.ViewModels.Main;
using IRis.Services;
using IRis.ViewModels.Circuit;
using IRis.Models.Core;
using IRis.ViewModels.Circuit.CircuitObjects;
using IRis.ViewModels.Circuit.Core;


namespace IRis.Views.Main;


public partial class MainCanvasView : UserControl
{
    public Simulation Simulation = Simulation.GetInstance();
    public Preview Preview = Preview.GetInstance();
    public Selection Selection = Selection.GetInstance();
    public Drag Drag = Drag.GetInstance();


    public MainCanvasView()
    {
        InitializeComponent();
        DataContext = new MainCanvasViewModel();
    }


    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        Preview.Show();
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        Preview.Hide();
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(sender as Control).Properties.IsLeftButtonPressed) return;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (Preview.HasObjects() && !Preview.HasNewWire()) 
            Preview.CommitAll();
        else
        {
            Selection.StartBox();
            e.Pointer.Capture(sender as Control);   // keeps focus till released
        }
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        Simulation.CurrentMousePos = SimulationService.SnapPointToGrid(
            e.GetPosition((Visual)sender!));

        if (Selection.IsVisible)
            Selection.UpdateBox(selectables: Simulation.Objects);
        else if (Preview.HasObjects())
            Preview.UpdatePosition(current: Simulation.CurrentMousePos);
        else if (Drag.HasObjects())
        {
            if (Selection.HasObjects()) 
                Selection.Ditch();

            Drag.UpdatePosition(current: Simulation.CurrentMousePos);
        }
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        e.Handled = true;

        if (Selection.IsVisible)
        {
            Selection.EndBox();
            e.Pointer.Capture(null);
        }
    }


    private void OnCircuitObjectPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
        var co = (sender as Control)!.DataContext as CircuitObjectViewModel;
        Selection.DitchPartial();

        if (co is ComponentViewModel c)
        {
            if (c.IsSelected) 
                Drag.StartWith(Selection.Objects);
            else
            {
                Selection.Focus(c);
                Drag.StartWith(c);
            }
        }
    }


    private void OnCircuitObjectPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        e.Handled = true;
        var co = ((sender as Control)!.DataContext as CircuitObjectViewModel)!;

        if (Drag.HasObjects())
        {
            if (Drag.Used)
                Selection.AddCollection(Drag.Objects);
            else
                Selection.Focus(co);

            Drag.End();
        }
    }


    private void OnCircuitObjectPointerEntered(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        if (Preview.HasObjects() || Drag.HasObjects()) return;
        if ((sender as Control)!.DataContext is not CircuitObjectViewModel co) return;

        if (!Selection.Objects.Contains(co))
            Selection.AddPartial(co);
    }


    private void OnCircuitObjectPointerExited(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        if (Preview.HasObjects() || Drag.HasObjects()) return;

        Selection.DitchPartial();
    }


    private void OnTerminalPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
        var t = ((sender as Control)!.DataContext as TerminalViewModel)!;
        Selection.DitchPartial();

        if (Preview.HasNewWire())
            Preview.EndWireAt(t);
        else
            Preview.StartWireAt(t);
    }


    private void OnTerminalPointerEntered(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        Selection.HidePartial();
    }


    private void OnTerminalPointerExited(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        Selection.ShowPartial();
    }


    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
