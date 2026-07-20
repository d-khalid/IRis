using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using IRis.Models;
using IRis.Models.CircuitObjects.Components.Gates;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;

public abstract partial class MultiInputGateViewModel : GateViewModel, IHaveDynamicPins
{
    public ObservableCollection<TerminalViewModel> Inputs { get; } = [];

    public MultiInputGateViewModel(MultiInputGate model)
        : base(model)
    {
        Inputs.CollectionChanged += (sender, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var vm = (e.NewItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Input;

                (Model as MultiInputGate)!.Inputs.Add(vm.GetModel());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vm = (e.OldItems![0] as TerminalViewModel)!;
                vm.Type = TerminalType.Input;

                (Model as MultiInputGate)!.Inputs.Remove(vm.GetModel());
            }

            Width = Height = Inputs.Count * 20;
        };
    }

    public void AddPin()
    {
        if (Inputs.Count < 50)
            Inputs.Add(new());
    }

    public void RemovePin()
    {
        if (Inputs.Count > 2)
            Inputs.Remove(Inputs[^1]);
    }

    protected override void UpdateInputTerminals()
    {
        double unrotatedX = X - 10;
        double multiplier = 20;

        for (int i = 0; i < Inputs.Count; i++)
        {
            double unrotatedY = Y + (i * multiplier) + 10;

            Point rotatedPos = _simulationService.RotateTerminalPosition(
                unrotatedX,
                unrotatedY,
                Rotation,
                Width,
                Height,
                X,
                Y
            );

            Inputs[i].X = rotatedPos.X;
            Inputs[i].Y = rotatedPos.Y;
        }
    }
}
