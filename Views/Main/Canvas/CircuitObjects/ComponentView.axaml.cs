using Avalonia.Controls;
using Avalonia.Input;
using IRis.ViewModels.Main.Canvas.CircuitObjects;


namespace IRis.Views.Main.Canvas.CircuitObjects;


public partial class ComponentView : UserControl
{
    public ComponentView()
    {
        InitializeComponent();
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        (DataContext as ComponentViewModel)!.PointerPressed();
    }
}
