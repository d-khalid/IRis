using Avalonia.Controls;
using IRis.ViewModels.Main;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.Views.Main;


public partial class LeftSidebarContent : UserControl
{
    public LeftSidebarContent()
    {
        InitializeComponent();
        DataContext = new LeftSidebarViewModel();
    }
}
