using Avalonia.Controls;
using IRis.ViewModels.Main;


namespace IRis.Views.Main;


public partial class TopMenuView : UserControl
{
    public TopMenuView()
    {
        InitializeComponent();
        DataContext = new TopMenuViewModel();
    }
}
