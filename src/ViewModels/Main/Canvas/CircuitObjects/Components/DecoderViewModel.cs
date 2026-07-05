using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;
using System;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class DecoderViewModel : ComponentViewModel
{
    public ObservableCollection<TerminalViewModel> Outputs { get; } = [];
    public ObservableCollection<TerminalViewModel> Selects { get; } = [];

    public DecoderViewModel()
        : this(new Decoder()) { }

    private DecoderViewModel(Decoder model)
        : base(model)
    {
        Outputs.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var vm = (e.NewItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Output;

                (Model as Decoder)!.Outputs.Add(vm.GetModel());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vm = (e.OldItems![0] as TerminalViewModel)!;
                (Model as Decoder)!.Outputs.Remove(vm.GetModel());
            }

            UpdateDimensions();
        };

        Selects.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var vm = (e.NewItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Input;

                (Model as Decoder)!.Selects.Add(vm.GetModel());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vm = (e.OldItems![0] as TerminalViewModel)!;
                (Model as Decoder)!.Selects.Remove(vm.GetModel());
            }

            UpdateDimensions();
        };
    }

    public void AddSelectLine()
    {
        Selects.Add(new TerminalViewModel());

        int target = (int)Math.Pow(2, Selects.Count);
        while (Outputs.Count < target)
            Outputs.Add(new TerminalViewModel());
    }

    public void RemoveSelectLine()
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
            Point selectPos = _simulationService.RotateTerminalPosition(
                X - 10,
                Y + ((i + 0.5) * (Height / Selects.Count)),
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
