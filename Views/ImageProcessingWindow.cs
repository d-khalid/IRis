// ImageProcessingWindow.axaml.cs

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using IRis.ViewModels;

namespace IRis.Views;

public partial class ImageProcessingWindow : Window
{
    public ImageProcessingWindow()
    {
        InitializeComponent();
        DataContext = new ImageProcessingWindowViewModel(this);
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}