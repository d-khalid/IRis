using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;
using Newtonsoft.Json;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class RegisterViewModel : ComponentViewModel, IHaveDynamicPins
{
    public ObservableCollection<TerminalViewModel> Inputs { get; } = [];
    public ObservableCollection<TerminalViewModel> Outputs { get; } = [];

    [ObservableProperty]
    private TerminalViewModel _clk = null!;

    partial void OnClkChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as Register)!.Clk = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _set = null!;

    partial void OnSetChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as Register)!.Set = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _clr = null!;

    partial void OnClrChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as Register)!.Clr = value.GetModel();
    }

    public List<LogicState> States
    {
        get => (Model as Register)!.States;
        set => (Model as Register)!.States = value;
    }

    public RegisterViewModel()
        : this(new Register()) { }

    private RegisterViewModel(Register model)
        : base(model)
    {
        Width = 40;

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

            UpdateDimensions();
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

            UpdateDimensions();
        };
    }

    public void AddPin()
    {
        if (Inputs.Count >= 50)
            return;

        Inputs.Add(new TerminalViewModel());
        Outputs.Add(new TerminalViewModel());
        (Model as Register)!.States.Add(LogicState.Unknown);
    }

    public void RemovePin()
    {
        if (Inputs.Count <= 1)
            return;

        Inputs.Remove(Inputs[^1]);
        Outputs.Remove(Outputs[^1]);

        var states = (Model as Register)!.States;
        if (states.Count > 0)
            states.RemoveAt(states.Count - 1);
    }

    private void UpdateDimensions()
    {
        if (Inputs.Count > 0)
            Height = (Inputs.Count * 20) + 20;
    }

    public override void UpdateTerminals()
    {
        if (Clk is null || Set is null || Clr is null)
            return;

        PlaceTerminal(Set, X + (Width / 2), Y);
        PlaceTerminal(Clk, X + (Width / 4), Y + Height + 10);
        PlaceTerminal(Clr, X + (3 * Width / 4), Y + Height);

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
