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
    private string _frequencyHzText;

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
        FrequencyHzText = Simulation.FrequencyHz.ToString();
    }

    partial void OnFrequencyHzTextChanged(string value)
    {
        if (int.TryParse(value, out int frequencyHz) && frequencyHz > 0 && frequencyHz <= 1000)
            Simulation.FrequencyHz = frequencyHz;
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

        _preview.Pick(gate, setVisible: false);
    }

    [RelayCommand]
    private void AddNot()
    {
        _selection.UnHighlightAll();

        NotGateViewModel gate = new() { Input = new(), Output = new() };
        _preview.Pick(gate, setVisible: false);
    }

    [RelayCommand]
    private void AddOr()
    {
        _selection.UnHighlightAll();

        OrGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        _preview.Pick(gate, setVisible: false);
    }

    [RelayCommand]
    private void AddXor()
    {
        _selection.UnHighlightAll();

        XorGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        _preview.Pick(gate, setVisible: false);
    }

    [RelayCommand]
    private void AddNand()
    {
        _selection.UnHighlightAll();

        NandGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        _preview.Pick(gate, setVisible: false);
    }

    [RelayCommand]
    private void AddNor()
    {
        _selection.UnHighlightAll();

        NorGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        _preview.Pick(gate, setVisible: false);
    }

    [RelayCommand]
    private void AddXnor()
    {
        _selection.UnHighlightAll();

        XnorGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        _preview.Pick(gate, setVisible: false);
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

        _preview.Pick(adder, setVisible: false);
    }

    [RelayCommand]
    private void AddDLatch()
    {
        _selection.UnHighlightAll();

        DLatchViewModel latch = new()
        {
            D = new(),
            En = new(),
            Q = new(),
            QBar = new(),
        };

        _preview.Pick(latch, setVisible: false);
    }

    [RelayCommand]
    private void AddDFlipFlop()
    {
        _selection.UnHighlightAll();

        DFlipFlopViewModel ff = new()
        {
            D = new(),
            Clk = new(),
            Set = new(),
            Clr = new(),
            Q = new(),
            QBar = new(),
        };

        _preview.Pick(ff, setVisible: false);
    }

    [RelayCommand]
    private void AddJKFlipFlop()
    {
        _selection.UnHighlightAll();

        JKFlipFlopViewModel ff = new()
        {
            J = new(),
            K = new(),
            Clk = new(),
            Set = new(),
            Clr = new(),
            Q = new(),
            QBar = new(),
        };

        _preview.Pick(ff, setVisible: false);
    }

    [RelayCommand]
    private void AddTFlipFlop()
    {
        _selection.UnHighlightAll();

        TFlipFlopViewModel ff = new()
        {
            T = new(),
            Clk = new(),
            Set = new(),
            Clr = new(),
            Q = new(),
            QBar = new(),
        };

        _preview.Pick(ff, setVisible: false);
    }

    [RelayCommand]
    private void AddRegister()
    {
        _selection.UnHighlightAll();

        RegisterViewModel reg = new()
        {
            Clk = new(),
            Set = new(),
            Clr = new(),
        };

        reg.AddBit();
        reg.AddBit();

        _preview.Pick(reg, setVisible: false);
    }

    [RelayCommand]
    private void AddCounter()
    {
        _selection.UnHighlightAll();

        CounterViewModel counter = new()
        {
            Clk = new(),
            Clr = new(),
            Load = new(),
            Enable = new(),
            Carry = new(),
        };

        counter.AddBit();
        counter.AddBit();

        _preview.Pick(counter, setVisible: false);
    }

    [RelayCommand]
    private void AddMultiplexer()
    {
        _selection.UnHighlightAll();

        MultiplexerViewModel mux = new() { Output = new() };
        mux.AddSelectLine();
        mux.AddSelectLine();

        _preview.Pick(mux, setVisible: false);
    }

    [RelayCommand]
    private void AddDemultiplexer()
    {
        _selection.UnHighlightAll();

        DemultiplexerViewModel demux = new() { Input = new() };
        demux.AddSelectLine();
        demux.AddSelectLine();

        _preview.Pick(demux, setVisible: false);
    }

    [RelayCommand]
    private void AddDecoder()
    {
        _selection.UnHighlightAll();

        DecoderViewModel decoder = new();
        decoder.AddSelectLine();
        decoder.AddSelectLine();

        _preview.Pick(decoder, setVisible: false);
    }

    [RelayCommand]
    private void AddPriorityEncoder()
    {
        _selection.UnHighlightAll();

        PriorityEncoderViewModel encoder = new();
        encoder.AddSelectLine();
        encoder.AddSelectLine();

        _preview.Pick(encoder, setVisible: false);
    }

    [RelayCommand]
    private void AddToggle()
    {
        _selection.UnHighlightAll();

        ToggleViewModel toggle = new() { Output = new() };
        _preview.Pick(toggle, setVisible: false);
    }

    [RelayCommand]
    private void AddClock()
    {
        _selection.UnHighlightAll();

        ClockViewModel clock = new() { Output = new() };
        _preview.Pick(clock, setVisible: false);
    }

    [RelayCommand]
    private void AddProbe()
    {
        _selection.UnHighlightAll();

        ProbeViewModel probe = new() { Input = new() };
        _preview.Pick(probe, setVisible: false);
    }

    [RelayCommand]
    private void AddTristateBuffer()
    {
        _selection.UnHighlightAll();

        TristateBufferViewModel tBuffer = new()
        {
            In = new(),
            Out = new(),
            En = new(),
        };
        _preview.Pick(tBuffer, setVisible: false);
    }
}
