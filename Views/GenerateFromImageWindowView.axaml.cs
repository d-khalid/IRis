using Avalonia.Controls;
using IRis.ViewModels;


namespace IRis.Views;


public partial class GenerateFromImageWindowView : Window
{
    public GenerateFromImageWindowView()
    {
        InitializeComponent();
        DataContext = new GenerateFromImageWindowViewModel(this);
    }
}
