using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Input;
using Avalonia.Controls;
using IRis.Services;
using IRis.Services.Singleton;
using Avalonia;
using System;
using Avalonia.Collections;
using IRis.ViewModels.Main.Canvas;


namespace IRis.ViewModels.Main;


public partial class CanvasViewModel : ViewModelBase
{
    [ObservableProperty] private AvaloniaList<CircuitObjectViewModel> _circuit = 
        Simulation.Get().Objects;
    [ObservableProperty] private Preview _preview = Preview.Get();
    [ObservableProperty] private SelectionBox _selectionBox = SelectionBox.Get();
    [ObservableProperty] private AppState _appState = AppState.Get();


    [RelayCommand]
    private static void Copy()
    {
        if (!Preview.Get().IsEmpty())
        {
            ClipboardService.Copy(Preview.Get().Objects);
            Preview.Get().Nuke();
        }

        else if (!Selection.Get().IsEmpty())
        {
            ClipboardService.Copy(Selection.Get().Objects);
            Selection.Get().UnHighlightAll();
        }
    }


    public void PointerEntered()
    {
        if (!Preview.Get().IsEmpty())
            Preview.Get().Show();
    }


    public void PointerExited()
    {
        if (!Preview.Get().IsEmpty())
            Preview.Get().Hide();
    }


    public static void PointerPressed(Control sender, PointerPressedEventArgs e)
    {
        if (!Preview.Get().IsEmpty() && !Preview.Get().HasNewWire())
            Preview.Get().Commit();
        else
        {
            SelectionBox.Get().StartAt(AppState.Get().MousePosition);
            e.Pointer.Capture(sender);   // keeps focus till released
        }
    }


    public void PointerMoved(object? sender, PointerEventArgs e)
    {
        AppState.Get().MousePosition = 
            SimulationService.SnapPointToGrid(e.GetPosition((Visual)sender!));

        if (SelectionBox.Get().Exists())
            SelectionBox.Get().UpdateTo(AppState.Get().MousePosition);

        else if (!Preview.Get().IsEmpty())
            Preview.Get().UpdatePositionTo(AppState.Get().MousePosition);

        else if (DragService.IsRunning())
            DragService.UpdatePositionTo(AppState.Get().MousePosition);
    }


    public static void PointerReleased(PointerReleasedEventArgs e)
    {
        if (SelectionBox.Get().Exists())
        {
            SelectionBox.Get().Nuke();
            e.Pointer.Capture(null);
        }
    }
}
