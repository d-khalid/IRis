using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using Microsoft.Extensions.Logging;

namespace IRis.ViewModels.Main;

public partial class LeftSidebarViewModel : ViewModelBase
{
    [ObservableProperty]
    private AppState _appState;

    [ObservableProperty]
    private Simulation _simulation;

    [ObservableProperty]
    private string _canvasWidth;

    [ObservableProperty]
    private string _canvasHeight;

    private readonly Selection _selection;
    private readonly Preview _preview;
    private readonly SimulationService _simulationService;
    private readonly ILogger<LeftSidebarViewModel> _logger;

    public LeftSidebarViewModel(
        AppState appState,
        Simulation simulation,
        Selection selection,
        Preview preview,
        SimulationService simulationService,
        ILogger<LeftSidebarViewModel> logger
    )
    {
        AppState = appState;
        _simulation = simulation;
        _selection = selection;
        _preview = preview;
        _simulationService = simulationService;
        _logger = logger;

        CanvasWidth = AppState.CanvasWidth.ToString();
        CanvasHeight = AppState.CanvasHeight.ToString();
    }

    partial void OnCanvasWidthChanged(string value)
    {
        if (
            int.TryParse(value, out int width)
            && width > 100
            && width < 25000
            && _simulationService.GetMaxPointInCollection(Simulation.Objects).X + 20 < width
        )
        {
            AppState.CanvasWidth = width;
        }
    }

    partial void OnCanvasHeightChanged(string value)
    {
        if (
            int.TryParse(value, out int height)
            && height > 100
            && height < 25000
            && _simulationService.GetMaxPointInCollection(Simulation.Objects).Y + 20 < height
        )
        {
            AppState.CanvasHeight = height;
        }
    }

    [RelayCommand]
    private void ShowDesignTab() => AppState.DesignTabActive = true;

    [RelayCommand]
    private void ShowCanvasTab() => AppState.DesignTabActive = false;

    [RelayCommand]
    private void AddAnd()
    {
        _selection.UnHighlightAll();

        AndGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        _preview.Pick(gate);
    }

    [RelayCommand]
    private void AddNot()
    {
        _selection.UnHighlightAll();

        NotGateViewModel gate = new() { Input = new(), Output = new() };
        _preview.Pick(gate);
    }

    [RelayCommand]
    private void AddOr()
    {
        _selection.UnHighlightAll();

        OrGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        _preview.Pick(gate);
    }

    [RelayCommand]
    private void AddXor()
    {
        _selection.UnHighlightAll();

        XorGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        _preview.Pick(gate);
    }

    [RelayCommand]
    private void AddNand()
    {
        _selection.UnHighlightAll();

        NandGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        _preview.Pick(gate);
    }

    [RelayCommand]
    private void AddNor()
    {
        _selection.UnHighlightAll();

        NorGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        _preview.Pick(gate);
    }

    [RelayCommand]
    private void AddXnor()
    {
        _selection.UnHighlightAll();

        XnorGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        _preview.Pick(gate);
    }

    [RelayCommand]
    private void AddFullAdder()
    {
        _selection.UnHighlightAll();

        FullAdderViewModel adder = new()
        {
            A = new(),
            B = new(),
            Cin = new(),
            Sum = new(),
            Cout = new(),
        };

        _preview.Pick(adder);
    }

    [RelayCommand]
    private void AddMultiplexer()
    {
        _selection.UnHighlightAll();

        MultiplexerViewModel mux = new() { Output = new() };
        mux.AddSelectLine();
        mux.AddSelectLine();

        _preview.Pick(mux);
    }

    [RelayCommand]
    private void AddDemultiplexer()
    {
        _selection.UnHighlightAll();

        DemultiplexerViewModel demux = new() { Input = new() };
        demux.AddSelectLine();
        demux.AddSelectLine();

        _preview.Pick(demux);
    }

    [RelayCommand]
    private void AddToggle()
    {
        _selection.UnHighlightAll();

        ToggleViewModel toggle = new() { Output = new() };
        _preview.Pick(toggle);
    }

    [RelayCommand]
    private void AddClock()
    {
        _selection.UnHighlightAll();

        ClockViewModel clock = new() { Output = new() };
        _preview.Pick(clock);
    }

    [RelayCommand]
    private void AddProbe()
    {
        _selection.UnHighlightAll();

        ProbeViewModel probe = new() { Input = new() };
        _preview.Pick(probe);
    }
}
