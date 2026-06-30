using Avalonia.Controls;
using IRis.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;

namespace IRis.Views.Main;

public partial class LeftSidebarContent : UserControl
{
    public LeftSidebarContent()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<LeftSidebarViewModel>();
    }
}
