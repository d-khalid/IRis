using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class MultiplexerViewModel : ComponentViewModel, IHaveDynamicPins
{
    public ObservableCollection<TerminalViewModel> Inputs { get; } = [];
    public ObservableCollection<TerminalViewModel> Selects { get; } = [];

    [ObservableProperty]
    private TerminalViewModel _output = null!;

    public MultiplexerViewModel()
        : this(new Multiplexer()) { }

    private MultiplexerViewModel(Multiplexer model)
        : base(model)
    {
        Inputs.CollectionChanged += (sender, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var vm = (e.NewItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Input;

                (Model as Multiplexer)!.Inputs.Add(vm.GetModel());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vm = (e.OldItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Input;

                (Model as Multiplexer)!.Inputs.Remove(vm.GetModel());
            }

            Height = Inputs.Count * 20;
            Width = Selects.Count * 20;
        };

        Selects.CollectionChanged += (sender, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var vm = (e.NewItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Input;

                (Model as Multiplexer)!.Selects.Add(vm.GetModel());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vm = (e.OldItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Input;

                (Model as Multiplexer)!.Selects.Remove(vm.GetModel());
            }

            Height = Inputs.Count * 20;
            Width = Selects.Count * 20;
        };
    }

    partial void OnOutputChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as Multiplexer)!.Output = value.GetModel();
    }

    public void AddPin()
    {
        if (Selects.Count >= 10)
            return;

        Selects.Add(new TerminalViewModel());
        while (Inputs.Count < Math.Pow(2, Selects.Count))
            Inputs.Add(new TerminalViewModel());
    }

    public void RemovePin()
    {
        if (Selects.Count <= 1)
            return;

        Selects.Remove(Selects[^1]);
        while (Inputs.Count > Math.Pow(2, Selects.Count))
            Inputs.Remove(Inputs[^1]);
    }

    public override void UpdateTerminals()
    {
        if (Output is null)
            return;

        Point outputPos = _simulationService.RotateTerminalPosition(
            X + (Width + 10),
            Y + (Height / 2),
            Rotation,
            Width,
            Height,
            X,
            Y
        );
        Output.X = outputPos.X;
        Output.Y = outputPos.Y;

        for (int i = 0; i < Inputs.Count; i++)
        {
            Point inputPos = _simulationService.RotateTerminalPosition(
                X - 10,
                Y + (i * 20) + 10,
                Rotation,
                Width,
                Height,
                X,
                Y
            );
            Inputs[i].X = inputPos.X;
            Inputs[i].Y = inputPos.Y;
        }

        for (int i = 0; i < Selects.Count; i++)
        {
            // DISCLAIMER: this calculation for placing the select lines on the
            // bottom of the trapezium was done by Cursor Grok 4.5
            // it works, so we keep it.

            double u = (i + 0.5) / Selects.Count;

            Point selectPos = _simulationService.RotateTerminalPosition(
                X + (u * Width),
                Y + (Height * (1.0 - (0.2 * u))),
                Rotation,
                Width,
                Height,
                X,
                Y
            );
            Selects[i].X = selectPos.X;
            Selects[i].Y = selectPos.Y;
        }
    }
}
