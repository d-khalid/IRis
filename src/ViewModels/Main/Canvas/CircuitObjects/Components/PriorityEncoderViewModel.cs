using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class PriorityEncoderViewModel : ComponentViewModel
{
    public ObservableCollection<TerminalViewModel> Inputs { get; } = [];
    public ObservableCollection<TerminalViewModel> Outputs { get; } = [];

    public PriorityEncoderViewModel()
        : this(new PriorityEncoder()) { }

    private PriorityEncoderViewModel(PriorityEncoder model)
        : base(model)
    {
        Inputs.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var vm = (e.NewItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Input;

                (Model as PriorityEncoder)!.Inputs.Add(vm.GetModel());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vm = (e.OldItems![0] as TerminalViewModel)!;
                (Model as PriorityEncoder)!.Inputs.Remove(vm.GetModel());
            }

            UpdateDimensions();
        };

        Outputs.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var vm = (e.NewItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Output;

                (Model as PriorityEncoder)!.Outputs.Add(vm.GetModel());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vm = (e.OldItems![0] as TerminalViewModel)!;
                (Model as PriorityEncoder)!.Outputs.Remove(vm.GetModel());
            }

            UpdateDimensions();
        };
    }

    public void AddSelectLine()
    {
        Outputs.Add(new TerminalViewModel());

        int target = (int)Math.Pow(2, Outputs.Count);
        while (Inputs.Count < target)
            Inputs.Add(new TerminalViewModel());
    }

    public void RemoveSelectLine()
    {
        if (Outputs.Count <= 1)
            return;

        Outputs.Remove(Outputs[^1]);

        int target = (int)Math.Pow(2, Outputs.Count);
        while (Inputs.Count > target)
            Inputs.Remove(Inputs[^1]);
    }

    private void UpdateDimensions()
    {
        if (Inputs.Count > 0)
            Height = Inputs.Count * 20;

        if (Outputs.Count > 0)
            Width = Outputs.Count * 20;
    }

    public override void UpdateTerminals()
    {
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

        for (int i = 0; i < Outputs.Count; i++)
        {
            Point outputPos = _simulationService.RotateTerminalPosition(
                X + (Width + 10),
                Y + ((i + 0.5) * (Height / Outputs.Count)),
                Rotation,
                Width,
                Height,
                X,
                Y
            );
            Outputs[i].X = outputPos.X;
            Outputs[i].Y = outputPos.Y;
        }
    }
}
