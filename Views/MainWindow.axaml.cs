using Avalonia.Controls;
using Avalonia.Input;
using IRis.Services;
using IRis.ViewModels;

namespace IRis.Views;

public partial class MainWindow : Window
{
    public MainWindow(CanvasService canvasService)
    {
        InitializeComponent();
        // "MainCanvas" from the XAML is used for all the drawing
        canvasService.Register(MainCanvas);
        
        
        
        
        // DataContext = MainWindowViewModel();
        
        // CircuitComponent component = new CircuitComponent();
        //     
        // MainCanvas.Children.Add(component);
    }
}