using Avalonia.Controls;
using Avalonia.Input;
using IRis.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;

namespace IRis.Views.Main;

public partial class RightSidebarView : UserControl
{
    public RightSidebarView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<RightSidebarViewModel>();
    }
}
