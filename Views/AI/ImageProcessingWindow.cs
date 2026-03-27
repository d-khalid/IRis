// ImageProcessingWindow.axaml.cs

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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