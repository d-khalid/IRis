using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;
using Avalonia.Input;
using Avalonia.Controls;
using IRis.Services;
using Avalonia;


namespace IRis.ViewModels.Main;


public partial class CanvasViewModel : ViewModelBase
{
    [ObservableProperty] private Simulation _simulation = Simulation.GetInstance();
    [ObservableProperty] private Preview _preview = Preview.GetInstance();
    [ObservableProperty] private Selection _selection = Selection.GetInstance();
    [ObservableProperty] private Drag _drag = Drag.GetInstance();
    public ClipboardManager Clipboard { get; } = ClipboardManager.GetInstance();


    [RelayCommand]
    private void Copy()
    {
        if (Preview.HasObjects())
        {
            Clipboard.Copy(Preview.Objects);
            Preview.Ditch();
        }
        else if (Selection.HasObjects())
        {
            Clipboard.Copy(Selection.Objects);
            Selection.Ditch();
        }
    }


    public void PointerEntered()
    {
        Preview.Show();
    }


    public void PointerExited()
    {
        Preview.Hide();
    }


    public void PointerPressed(object? sender, PointerPressedEventArgs e)
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


    public void PointerMoved(object? sender, PointerEventArgs e)
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


    public void PointerReleased(PointerReleasedEventArgs e)
    {
        e.Handled = true;

        if (Selection.IsVisible)
        {
            Selection.EndBox();
            e.Pointer.Capture(null);
        }
    }
}
