using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using IRis.Services;
using IRis.Services.Singleton;

namespace IRis.ViewModels.Main;

public partial class CanvasViewModel(
    Simulation simulation,
    Preview preview,
    SelectionBox selectionBox,
    WirePreview wirePreview,
    AppState appState,
    DragService dragService,
    SimulationService simulationService,
    HoverEffectService hoverEffectService
) : ViewModelBase
{
    public Preview Preview { get; } = preview;
    public SelectionBox SelectionBox { get; } = selectionBox;
    public WirePreview WirePreview { get; } = wirePreview;
    public Simulation Simulation { get; } = simulation;
    public AppState AppState { get; } = appState;

    private readonly DragService _dragService = dragService;
    private readonly SimulationService _simulationService = simulationService;
    private readonly HoverEffectService _hoverEffectService = hoverEffectService;

    public void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        e.Handled = true;

        if (!AppState.EditingAllowed)
            return;

        if (!Preview.IsEmpty())
            Preview.Show();

        if (!WirePreview.IsEmpty())
            WirePreview.Show();
    }

    public void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        e.Handled = true;

        if (!AppState.EditingAllowed)
            return;

        if (!Preview.IsEmpty())
            Preview.Hide();

        if (!WirePreview.IsEmpty())
            WirePreview.Hide();
    }

    public void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            e.Pointer.Capture(null);
            return;
        }

        if (!e.GetCurrentPoint(sender as Control).Properties.IsLeftButtonPressed)
            return;

        e.Handled = true;
        AppState.MousePosition = _simulationService.SnapPointToGrid(e.GetPosition((Visual)sender!));

        if (!AppState.EditingAllowed)
            return;

        if (!WirePreview.IsEmpty())
            WirePreview.Checkpoint();
        else if (!Preview.IsEmpty())
            Preview.Commit();
        else
        {
            SelectionBox.StartAt(AppState.MousePosition);
            e.Pointer.Capture(sender as Control); // keeps focus till released
        }
    }

    public void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        AppState.MousePosition = _simulationService.SnapPointToGrid(e.GetPosition((Visual)sender!));

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        e.Handled = true;

        if (!AppState.EditingAllowed)
            return;

        if (SelectionBox.Exists())
            SelectionBox.UpdateTo(AppState.MousePosition);
        else if (!Preview.IsEmpty())
            Preview.UpdatePositionTo(AppState.MousePosition);
        else if (!WirePreview.IsEmpty())
            WirePreview.UpdateTo(AppState.MousePosition);
        else if (_dragService.IsRunning())
            _dragService.UpdatePositionTo(AppState.MousePosition);
    }

    public void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        e.Pointer.Capture(null);

        if (SelectionBox.Exists())
            SelectionBox.Nuke();

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        e.Handled = true;

        if (!AppState.EditingAllowed)
            return;
    }

    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        double step = AppState.PanSensistivity * 10;
        Point mousePosition = AppState.MousePosition;

        var zoomBorder = (sender as ZoomBorder)!;
        var m = zoomBorder.Matrix;
        double delta = step / m.M11; // normalize step to zoom scale (for MousePosition)

        switch (e.Key)
        {
            case Key.Left:
                zoomBorder.SetMatrix(new Matrix(m.M11, m.M12, m.M21, m.M22, m.M31 + step, m.M32));
                mousePosition = new(mousePosition.X - delta, mousePosition.Y);
                break;

            case Key.Right:
                zoomBorder.SetMatrix(new Matrix(m.M11, m.M12, m.M21, m.M22, m.M31 - step, m.M32));
                mousePosition = new(mousePosition.X + delta, mousePosition.Y);
                break;

            case Key.Up:
                zoomBorder.SetMatrix(new Matrix(m.M11, m.M12, m.M21, m.M22, m.M31, m.M32 + step));
                mousePosition = new(mousePosition.X, mousePosition.Y - delta);
                break;

            case Key.Down:
                zoomBorder.SetMatrix(new Matrix(m.M11, m.M12, m.M21, m.M22, m.M31, m.M32 - step));
                mousePosition = new(mousePosition.X, mousePosition.Y + delta);
                break;

            default:
                return;
        }

        AppState.MousePosition = _simulationService.SnapPointToGrid(mousePosition);
        e.Handled = true;
    }
}
