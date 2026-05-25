using Avalonia.Controls;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;


namespace IRis.Views.Main.Canvas.CircuitObjects.Components;


public partial class GateView : UserControl
{
    public GateView()
    {
        InitializeComponent();
        DataContext = new GateViewModel();
    }
}
