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
        Simulation.GetInstance().Objects;
    [ObservableProperty] private Preview _preview = Preview.GetInstance();
    [ObservableProperty] private SelectionBox _selectionBox = SelectionBox.GetInstance();
    [ObservableProperty] private AppState _appState = AppState.GetInstance();


    [RelayCommand]
    private static void Copy()
    {
        if (!Preview.GetInstance().IsEmpty())
        {
            ClipboardService.Copy(Preview.GetInstance().Objects);
            Preview.GetInstance().Nuke();
        }

        else if (!Selection.GetInstance().IsEmpty())
        {
            ClipboardService.Copy(Selection.GetInstance().Objects);
            Selection.GetInstance().UnHighlightAll();
        }
    }


    public void PointerEntered()
    {
        if (!Preview.GetInstance().IsEmpty())
            Preview.GetInstance().Show();
    }


    public void PointerExited()
    {
        if (!Preview.GetInstance().IsEmpty())
            Preview.GetInstance().Hide();
    }


    public static void PointerPressed(Control sender, PointerPressedEventArgs e)
    {
        if (!Preview.GetInstance().IsEmpty() && !Preview.GetInstance().HasNewWire())
            Preview.GetInstance().Commit();
        else
        {
            SelectionBox.GetInstance().StartAt(AppState.GetInstance().MousePosition);
            e.Pointer.Capture(sender);   // keeps focus till released
        }
    }


    public void PointerMoved(object? sender, PointerEventArgs e)
    {
        AppState.GetInstance().MousePosition = 
            SimulationService.SnapPointToGrid(e.GetPosition((Visual)sender!));

        if (SelectionBox.GetInstance().Exists())
            SelectionBox.GetInstance().UpdateTo(AppState.GetInstance().MousePosition);

        else if (!Preview.GetInstance().IsEmpty())
            Preview.GetInstance().UpdatePositionTo(AppState.GetInstance().MousePosition);

        else if (DragService.IsRunning())
            DragService.UpdatePositionTo(AppState.GetInstance().MousePosition);
    }


    public static void PointerReleased(PointerReleasedEventArgs e)
    {
        if (SelectionBox.GetInstance().Exists())
        {
            SelectionBox.GetInstance().Nuke();
            e.Pointer.Capture(null);
        }
    }
}
