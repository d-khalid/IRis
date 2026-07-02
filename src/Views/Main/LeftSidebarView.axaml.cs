using Avalonia.Controls;
using IRis.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;

namespace IRis.Views.Main;

public partial class LeftSidebarView : UserControl
{
    public LeftSidebarView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<LeftSidebarViewModel>();
    }
}
