using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.Services.Singleton;

namespace IRis.ViewModels.Main;

public partial class CanvasViewModel : ViewModelBase
{
    [ObservableProperty]
    private Preview _preview;

    [ObservableProperty]
    private SelectionBox _selectionBox;

    [ObservableProperty]
    private AppState _appState;

    [ObservableProperty]
    private WirePreview _wirePreview;

    [ObservableProperty]
    private Simulation _simulation;

    private readonly Selection _selection;

    public CanvasViewModel(
        Simulation simulation,
        Preview preview,
        SelectionBox selectionBox,
        WirePreview wirePreview,
        AppState appState,
        Selection selection
    )
    {
        Simulation = simulation;
        Preview = preview;
        SelectionBox = selectionBox;
        WirePreview = wirePreview;
        AppState = appState;
        _selection = selection;
    }

    [RelayCommand]
    private void Copy()
    {
        if (!Preview.IsEmpty())
        {
            ClipboardService.Copy(Preview.Objects);
            Preview.Nuke();
        }
        else if (!_selection.IsEmpty())
        {
            ClipboardService.Copy(_selection.Objects);
            _selection.UnHighlightAll();
        }
    }

    public void PointerEntered()
    {
        if (!Preview.IsEmpty())
            Preview.Show();

        if (!WirePreview.IsEmpty())
            WirePreview.Show();
    }

    public void PointerExited()
    {
        if (!Preview.IsEmpty())
            Preview.Hide();

        if (!WirePreview.IsEmpty())
            WirePreview.Hide();
    }

    public void PointerPressed(Control sender, PointerPressedEventArgs e)
    {
        if (!WirePreview.IsEmpty())
            WirePreview.Checkpoint();
        else if (!Preview.IsEmpty())
            Preview.Commit();
        else
        {
            SelectionBox.StartAt(AppState.MousePosition);
            e.Pointer.Capture(sender); // keeps focus till released
        }
    }

    public void PointerMoved(object? sender, PointerEventArgs e)
    {
        AppState.MousePosition = SimulationService.SnapPointToGrid(e.GetPosition((Visual)sender!));

        if (SelectionBox.Exists())
            SelectionBox.UpdateTo(AppState.MousePosition);
        else if (!Preview.IsEmpty())
            Preview.UpdatePositionTo(AppState.MousePosition);
        else if (!WirePreview.IsEmpty())
            WirePreview.UpdateTo(AppState.MousePosition);
        else if (DragService.IsRunning())
            DragService.UpdatePositionTo(AppState.MousePosition);
    }

    public void PointerReleased(PointerReleasedEventArgs e)
    {
        if (SelectionBox.Exists())
        {
            SelectionBox.Nuke();
            e.Pointer.Capture(null);
        }
    }
}
