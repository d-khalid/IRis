using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.ViewModels;
using IRis.ViewModels.Main;


namespace IRis.Views.Main;


public partial class BottomStatusBarView : UserControl
{
    public BottomStatusBarView()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = new BottomStatusBarViewModel();
    }
}
