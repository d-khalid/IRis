using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.Services.Singleton;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.ViewModels.Main.Canvas.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Media;


namespace IRis.ViewModels.Main;


public partial class LeftSidebarViewModel : ViewModelBase
{
    [ObservableProperty]
    private AppState _appState = AppState.Get();

    [ObservableProperty]
    private string _simulationToggleContent = Simulation.Get().Running ?
        "Simulation: ON" : "Simulation: OFF";

    [ObservableProperty]
    private Brush _simulationToggleBrush = new SolidColorBrush(
        Simulation.Get().Running ? Colors.DarkGreen : Colors.DarkRed
    );


    public LeftSidebarViewModel()
    {
        Simulation.Get().PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(Simulation.Running))
            {
                SimulationToggleContent = Simulation.Get().Running ?
                    "Simulation: ON" : "Simulation: OFF";

                SimulationToggleBrush = new SolidColorBrush(
                    Simulation.Get().Running ? Colors.DarkGreen : Colors.DarkRed
                );
            }
        };
    }


    [RelayCommand]
    private static void SimulationToggle()
    {
        if (!Simulation.Get().Running) Simulation.Get().Running = true;
        else Simulation.Get().Running = false;
    }


    [RelayCommand]
    private static void ShowDesignTab() => AppState.Get().DesignTabActive = true;


    [RelayCommand]
    private static void ShowSimulateTab() => AppState.Get().DesignTabActive = false;


    [RelayCommand]
    private static void AddAnd()
    {
        Selection.Get().UnHighlightAll();

        AndGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        Preview.Get().Pick(gate);
    }


    [RelayCommand]
    private static void AddNot()
    {
        Selection.Get().UnHighlightAll();

        NotGateViewModel gate = new() { Input = new(), Output = new() };
        Preview.Get().Pick(gate);
    }


    [RelayCommand]
    private static void AddOr()
    {
        Selection.Get().UnHighlightAll();

        OrGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        Preview.Get().Pick(gate);
    }


    [RelayCommand]
    private static void AddXor()
    {
        Selection.Get().UnHighlightAll();

        XorGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        Preview.Get().Pick(gate);
    }


    [RelayCommand]
    private static void AddNand()
    {
        Selection.Get().UnHighlightAll();

        NandGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        Preview.Get().Pick(gate);
    }


    [RelayCommand]
    private static void AddNor()
    {
        Selection.Get().UnHighlightAll();

        NorGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        Preview.Get().Pick(gate);
    }


    [RelayCommand]
    private static void AddXnor()
    {
        Selection.Get().UnHighlightAll();

        XnorGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        Preview.Get().Pick(gate);
    }


    [RelayCommand]
    private static void AddFullAdder()
    {
        Selection.Get().UnHighlightAll();

        FullAdderViewModel adder = new()
        {
            A = new(),
            B = new(),
            Cin = new(),
            Sum = new(),
            Cout = new()
        };

        Preview.Get().Pick(adder);
    }


    [RelayCommand]
    private static void AddMultiplexer()
    {
        Selection.Get().UnHighlightAll();

        MultiplexerViewModel mux = new() { Output = new() };
        mux.AddSelectLine();
        mux.AddSelectLine();

        Preview.Get().Pick(mux);
    }


    [RelayCommand]
    private static void AddDemultiplexer()
    {
        Selection.Get().UnHighlightAll();

        DemultiplexerViewModel demux = new() { Input = new() };
        demux.AddSelectLine();
        demux.AddSelectLine();

        Preview.Get().Pick(demux);
    }


    [RelayCommand]
    private static void AddToggle()
    {
        Selection.Get().UnHighlightAll();

        ToggleViewModel toggle = new() { Output = new() };
        Preview.Get().Pick(toggle);
    }


    [RelayCommand]
    private static void AddProbe()
    {
        Selection.Get().UnHighlightAll();

        ProbeViewModel probe = new() { Input = new() };
        Preview.Get().Pick(probe);
    }
}
