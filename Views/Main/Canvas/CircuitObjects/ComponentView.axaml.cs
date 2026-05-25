using Avalonia.Controls;
using IRis.ViewModels.Main.Canvas;


namespace IRis.Views.Main.Canvas.CircuitObjects;


public partial class ComponentView : UserControl
{
    public ComponentView()
    {
        InitializeComponent();
        DataContext = new ComponentViewModel();
    }
}
