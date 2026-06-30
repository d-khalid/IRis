using Avalonia.Controls;
using Avalonia.Input;
using IRis.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace IRis.Views;

public partial class MainWindowView : Window
{
    public MainWindowView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<MainWindowViewModel>();
    }

    private void OnTitleBarPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }
}
