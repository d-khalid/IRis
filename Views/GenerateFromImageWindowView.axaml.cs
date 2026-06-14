using Avalonia.Controls;
using Avalonia.Input;
using IRis.ViewModels;


namespace IRis.Views;


public partial class GenerateFromImageWindowView : Window
{
    public GenerateFromImageWindowView()
    {
        InitializeComponent();
        DataContext = new GenerateFromImageWindowViewModel();
    }
}
