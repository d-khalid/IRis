using Avalonia.Controls;
using IRis.ViewModels.Main;


namespace IRis.Views.Main;


public partial class LeftSidebarView : UserControl
{
    public LeftSidebarView()
    {
        InitializeComponent();
        DataContext = new LeftSidebarViewModel();
    }
}
