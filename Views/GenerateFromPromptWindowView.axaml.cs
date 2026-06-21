using Avalonia.Controls;
using IRis.ViewModels;


namespace IRis.Views;


public partial class GenerateFromPromptWindowView : Window
{
    public GenerateFromPromptWindowView()
    {
        InitializeComponent();
        DataContext = new GenerateFromPromptWindowViewModel(this);
    }
}
