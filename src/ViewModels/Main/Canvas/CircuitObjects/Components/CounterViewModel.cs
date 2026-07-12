using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class CounterViewModel : ComponentViewModel
{
    public ObservableCollection<TerminalViewModel> Inputs { get; } = [];
    public ObservableCollection<TerminalViewModel> Outputs { get; } = [];

    [ObservableProperty]
    private TerminalViewModel _clk = null!;

    [ObservableProperty]
    private TerminalViewModel _clr = null!;

    [ObservableProperty]
    private TerminalViewModel _load = null!;

    [ObservableProperty]
    private TerminalViewModel _enable = null!;

    [ObservableProperty]
    private TerminalViewModel _carry = null!;

    public CounterViewModel()
        : this(new Counter()) { }

    private CounterViewModel(Counter model)
        : base(model)
    {
        Width = 80;

        Inputs.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var vm = (e.NewItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Input;
                model.Inputs.Add(vm.GetModel());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vm = (e.OldItems![0] as TerminalViewModel)!;
                model.Inputs.Remove(vm.GetModel());
            }

            Height = (Inputs.Count * 20) + 20;
        };

        Outputs.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var vm = (e.NewItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Output;
                model.Outputs.Add(vm.GetModel());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vm = (e.OldItems![0] as TerminalViewModel)!;
                model.Outputs.Remove(vm.GetModel());
            }

            Height = (Inputs.Count * 20) + 20;
        };
    }

    partial void OnClkChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as Counter)!.Clk = value.GetModel();
    }

    partial void OnClrChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as Counter)!.Clr = value.GetModel();
    }

    partial void OnLoadChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as Counter)!.Load = value.GetModel();
    }

    partial void OnEnableChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as Counter)!.Enable = value.GetModel();
    }

    partial void OnCarryChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as Counter)!.Carry = value.GetModel();
    }

    public void AddBit()
    {
        if (Inputs.Count >= 50)
            return;

        Inputs.Add(new TerminalViewModel());
        Outputs.Add(new TerminalViewModel());
        (Model as Counter)!.States.Add(LogicState.Unknown);
    }

    public void RemoveBit()
    {
        if (Inputs.Count <= 1)
            return;

        Inputs.Remove(Inputs[^1]);
        Outputs.Remove(Outputs[^1]);

        var states = (Model as Counter)!.States;
        if (states.Count > 0)
            states.RemoveAt(states.Count - 1);
    }

    public override void UpdateTerminals()
    {
        if (Clk is null || Clr is null || Load is null || Enable is null || Carry is null)
            return;

        PlaceTerminal(Carry, X + (Width / 2), Y - 10);

        PlaceTerminal(Clk, X + 10, Y + Height + 10);
        PlaceTerminal(Enable, X + 30, Y + Height);
        PlaceTerminal(Load, X + 50, Y + Height);
        PlaceTerminal(Clr, X + 70, Y + Height);

        for (int i = 0; i < Inputs.Count; i++)
            PlaceTerminal(Inputs[i], X - 10, Y + (i * 20) + 20);

        for (int i = 0; i < Outputs.Count; i++)
            PlaceTerminal(Outputs[i], X + Width + 10, Y + (i * 20) + 20);
    }

    private void PlaceTerminal(TerminalViewModel terminal, double unrotatedX, double unrotatedY)
    {
        Point rotatedPos = _simulationService.RotateTerminalPosition(
            unrotatedX,
            unrotatedY,
            Rotation,
            Width,
            Height,
            X,
            Y
        );

        terminal.X = rotatedPos.X;
        terminal.Y = rotatedPos.Y;
    }
}
