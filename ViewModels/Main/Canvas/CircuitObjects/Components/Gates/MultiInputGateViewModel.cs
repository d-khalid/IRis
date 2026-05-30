using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using IRis.Models.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.Models.Main.Canvas.Core;
using IRis.Services;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;


public abstract partial class MultiInputGateViewModel : GateViewModel
{
    public ObservableCollection<TerminalViewModel> Inputs { get; } = [];


    public MultiInputGateViewModel(MultiInputGate model) : base(model)
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


    protected override void UpdateInputTerminals()
    {
        double unrotatedX = X - 10;
        double multiplier = 20;

        for (int i = 0; i < Inputs.Count; i++)
        {
            double unrotatedY = Y + (i * multiplier) + 10;

            Point rotatedPos = SimulationService.RotateTerminalPosition(
                unrotatedX, unrotatedY, Rotation, Width, Height, X, Y
            );

            Inputs[i].X = rotatedPos.X;
            Inputs[i].Y = rotatedPos.Y;
        }
    }
}
