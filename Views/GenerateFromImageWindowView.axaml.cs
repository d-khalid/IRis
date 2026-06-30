using Avalonia.Controls;
using IRis.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace IRis.Views;

public partial class GenerateFromImageWindowView : Window
{
    public GenerateFromImageWindowView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<GenerateFromImageWindowViewModel>();
    }
}
