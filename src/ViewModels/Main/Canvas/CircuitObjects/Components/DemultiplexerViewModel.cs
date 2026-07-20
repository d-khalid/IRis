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

public partial class DemultiplexerViewModel : ComponentViewModel, IHaveDynamicPins
{
    public ObservableCollection<TerminalViewModel> Outputs { get; } = [];
    public ObservableCollection<TerminalViewModel> Selects { get; } = [];

    [ObservableProperty]
    private TerminalViewModel _input = null!;

    partial void OnInputChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as Demultiplexer)!.Input = value.GetModel();
    }

    public DemultiplexerViewModel()
        : this(new Demultiplexer()) { }

    private DemultiplexerViewModel(Demultiplexer model)
        : base(model)
    {
        Outputs.CollectionChanged += (sender, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var vm = (e.NewItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Output;

                (Model as Demultiplexer)!.Outputs.Add(vm.GetModel());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vm = (e.OldItems![0] as TerminalViewModel)!;
                (Model as Demultiplexer)!.Outputs.Remove(vm.GetModel());
            }

            UpdateDimensions();
        };

        Selects.CollectionChanged += (sender, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var vm = (e.NewItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Input;

                (Model as Demultiplexer)!.Selects.Add(vm.GetModel());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vm = (e.OldItems![0] as TerminalViewModel)!;
                (Model as Demultiplexer)!.Selects.Remove(vm.GetModel());
            }

            UpdateDimensions();
        };
    }

    public void AddPin()
    {
        Selects.Add(new TerminalViewModel());

        int target = (int)Math.Pow(2, Selects.Count);
        while (Outputs.Count < target)
            Outputs.Add(new TerminalViewModel());
    }

    public void RemovePin()
    {
        if (Selects.Count <= 1)
            return;

        Selects.Remove(Selects[^1]);

        int target = (int)Math.Pow(2, Selects.Count);
        while (Outputs.Count > target)
            Outputs.Remove(Outputs[^1]);
    }

    private void UpdateDimensions()
    {
        if (Outputs.Count > 0)
            Height = Outputs.Count * 20;

        if (Selects.Count > 0)
            Width = Selects.Count * 20;
    }

    public override void UpdateTerminals()
    {
        if (Input is null)
            return;

        Point inputPos = _simulationService.RotateTerminalPosition(
            X - 10,
            Y + (Height / 2),
            Rotation,
            Width,
            Height,
            X,
            Y
        );
        Input.X = inputPos.X;
        Input.Y = inputPos.Y;

        for (int i = 0; i < Outputs.Count; i++)
        {
            Point outputPos = _simulationService.RotateTerminalPosition(
                X + (Width + 10),
                Y + (i * 20) + 10,
                Rotation,
                Width,
                Height,
                X,
                Y
            );
            Outputs[i].X = outputPos.X;
            Outputs[i].Y = outputPos.Y;
        }

        for (int i = 0; i < Selects.Count; i++)
        {
            // DISCLAIMER: this calculation for placing the select lines on the
            // bottom of the trapezium was done by Cursor Grok 4.5
            // it works, so we keep it.

            double u = (i + 0.5) / Selects.Count;

            Point selectPos = _simulationService.RotateTerminalPosition(
                X + (u * Width),
                Y + (Height * (0.8 + (0.2 * u))),
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
